using Dislinkt.Saga.Data;
using Dislinkt.Saga.Proxy;

namespace Dislinkt.Saga.Menager
{
    public class RegistrationMenager:IRegistrationMenager
    {
        private readonly IProfileProxy profileProxy;
        private readonly IConnectionProxy connectionProxy;
        private readonly INotificationProxy notificationProxy;


        enum RegistrationTransactionState
        {
            NotStarted,
            ProfileCreated,
            ProfileCancelled,
            ProfileCreateFailed,
            ConnectionCreated,
            ConnectionCreatedFailed,
            ConnectionRolleback,
            NotificationCreated,
            NotificationCancelled,
            NotificationRollback
        }

        enum RegistrationAction
        {
            CreateProfile,
            CancelProfile,
            CreateConnection,
            RollbackConnection,
            CreateNotification,
            RollbackNotification
        }

        public RegistrationMenager(IProfileProxy profileProxy,IConnectionProxy connectionProxy,INotificationProxy notificationProxy)
        {
            this.profileProxy = profileProxy;
            this.notificationProxy= notificationProxy;
            this.connectionProxy= connectionProxy;
        }

        public bool Register(UserData input)
        {
            var registrationStateMachine = new Stateless.StateMachine<RegistrationTransactionState, RegistrationAction>(
                RegistrationTransactionState.NotStarted);

            var user = new User();
            var isSuccess = false;
            registrationStateMachine.Configure(RegistrationTransactionState.NotStarted)
                .PermitDynamic(RegistrationAction.CreateProfile, () => {
                    (user, isSuccess) = profileProxy.CreateUser(input).Result;
                    return isSuccess ? RegistrationTransactionState.ProfileCreated : RegistrationTransactionState.ProfileCreateFailed;
                });

            registrationStateMachine.Configure(RegistrationTransactionState.ProfileCreated)
                  .PermitDynamic(RegistrationAction.CreateConnection, () => {
                       (user, isSuccess) = connectionProxy.CreateNode(user).Result;
                      return isSuccess ? RegistrationTransactionState.ConnectionCreated : RegistrationTransactionState.ConnectionCreatedFailed;
                  })
                  .OnEntry(()=>registrationStateMachine.Fire(RegistrationAction.CreateConnection));

            registrationStateMachine.Configure(RegistrationTransactionState.ConnectionCreated)
                 .PermitDynamic(RegistrationAction.CreateNotification, () => {
                     (user, isSuccess) = notificationProxy.CreateNotificationSetting(user).Result;
                     return isSuccess ? RegistrationTransactionState.NotificationCreated : RegistrationTransactionState.NotificationCancelled;
                 })
                 .OnEntry(() => registrationStateMachine.Fire(RegistrationAction.CreateNotification));

            registrationStateMachine.Configure(RegistrationTransactionState.ConnectionCreatedFailed)
                .PermitDynamic(RegistrationAction.RollbackConnection, () =>
                {
                    connectionProxy.DeleteNodeAsync(user);
                    return RegistrationTransactionState.ConnectionRolleback;
                })
                .OnEntry(() => registrationStateMachine.Fire(RegistrationAction.RollbackConnection));


            registrationStateMachine.Configure(RegistrationTransactionState.ConnectionRolleback)
                .PermitDynamic(RegistrationAction.CancelProfile, () =>
                {
                    profileProxy.DeleteProfileAsync(user);
                    return RegistrationTransactionState.ProfileCancelled;
                })
                .OnEntry(() => registrationStateMachine.Fire(RegistrationAction.CancelProfile));



            registrationStateMachine.Fire(RegistrationAction.CreateProfile);
            return registrationStateMachine.State == RegistrationTransactionState.NotificationCreated;
            
        }

    }
}
