using Dislinkt.Saga.Data;
using Dislinkt.Saga.Proxy;

namespace Dislinkt.Saga.Menager
{
    public class RegistrationMenager:IRegistrationMenager
    {
        private readonly IProfileProxy profileProxy;
        private readonly IConnectionProxy connectionProxy;
        private readonly INotificationProxy notificationProxy;
        private readonly IActivityProxy activityProxy;


        enum RegistrationTransactionState
        {
            NotStarted,
            ProfileCreated,
            ProfileCancelled,
            ProfileRollback,
            ProfileCreateFailed,
            ConnectionCreated,
            ConnectionCreatedFailed,
            ConnectionRolleback,
            NotificationCreated,
            NotificationCareateFailed,
            NotificationRollback,
            ActivityCreated,
            ActivityCreatedFailed,
            ActivityRolleback
        }

        enum RegistrationAction
        {
            CreateProfile,
            RollbackProfile,
            CancelProfile,
            CreateConnection,
            RollbackConnection,
            CreateNotification,
            RollbackNotification,
            CreateActivity,
            RollbackActivity
        }

        public RegistrationMenager(IProfileProxy profileProxy,IConnectionProxy connectionProxy,INotificationProxy notificationProxy,IActivityProxy activityProxy)
        {
            this.profileProxy = profileProxy;
            this.notificationProxy= notificationProxy;
            this.connectionProxy= connectionProxy;
            this.activityProxy = activityProxy;
        }

        public bool Register(UserData input)
        {
            var registrationStateMachine = new Stateless.StateMachine<RegistrationTransactionState, RegistrationAction>(
                RegistrationTransactionState.NotStarted);

            var user = new User();
            var user1 = new User();
            var isSuccess = false;
            registrationStateMachine.Configure(RegistrationTransactionState.NotStarted)
                .PermitDynamic(RegistrationAction.CreateProfile, () => {
                    (user, isSuccess) = profileProxy.CreateUser(input).Result;
                    return isSuccess ? RegistrationTransactionState.ProfileCreated : RegistrationTransactionState.ProfileCreateFailed;
                });

            registrationStateMachine.Configure(RegistrationTransactionState.ProfileCreated)
                  .PermitDynamic(RegistrationAction.CreateConnection, () => {
                       (user1, isSuccess) = connectionProxy.CreateNode(user).Result;
                      return isSuccess ? RegistrationTransactionState.ConnectionCreated : RegistrationTransactionState.ConnectionCreatedFailed;
                  })
                  .OnEntry(()=>registrationStateMachine.Fire(RegistrationAction.CreateConnection));

            registrationStateMachine.Configure(RegistrationTransactionState.ConnectionCreated)
                 .PermitDynamic(RegistrationAction.CreateNotification, () => {
                     (user1, isSuccess) = notificationProxy.CreateNotificationSetting(user).Result;
                     return isSuccess ? RegistrationTransactionState.NotificationCreated : RegistrationTransactionState.NotificationCareateFailed;
                 })
                 .OnEntry(() => registrationStateMachine.Fire(RegistrationAction.CreateNotification));

            registrationStateMachine.Configure(RegistrationTransactionState.NotificationCreated)
              .PermitDynamic(RegistrationAction.CreateActivity, () => {
                  (user1, isSuccess) = activityProxy.CreateActivity(user).Result;
                  return isSuccess ? RegistrationTransactionState.ActivityCreated : RegistrationTransactionState.ActivityCreatedFailed;
              })
              .OnEntry(() => registrationStateMachine.Fire(RegistrationAction.CreateActivity));

            //rolback
            registrationStateMachine.Configure(RegistrationTransactionState.ActivityCreatedFailed)
               .PermitDynamic(RegistrationAction.RollbackActivity, () =>
               {
                  // notificationProxy.DeleteNotificationAsync(user);
                   return RegistrationTransactionState.ActivityRolleback;
               })
               .OnEntry(() => registrationStateMachine.Fire(RegistrationAction.RollbackActivity));

            registrationStateMachine.Configure(RegistrationTransactionState.NotificationCareateFailed)
            .PermitDynamic(RegistrationAction.RollbackNotification, () =>
            {
                   // notificationProxy.DeleteNotificationAsync(user);
                return RegistrationTransactionState.NotificationRollback;
            })
            .OnEntry(() => registrationStateMachine.Fire(RegistrationAction.RollbackNotification));

            registrationStateMachine.Configure(RegistrationTransactionState.ConnectionCreatedFailed)
          .PermitDynamic(RegistrationAction.RollbackConnection, () =>
          {
                // notificationProxy.DeleteNotificationAsync(user);
              return RegistrationTransactionState.ConnectionRolleback;
          })
          .OnEntry(() => registrationStateMachine.Fire(RegistrationAction.RollbackConnection));

            //nastavak 

          registrationStateMachine.Configure(RegistrationTransactionState.ActivityRolleback)
          .PermitDynamic(RegistrationAction.RollbackNotification, () =>
          {
               notificationProxy.DeleteNotificationAsync(user);
              return RegistrationTransactionState.NotificationRollback;
          })
          .OnEntry(() => registrationStateMachine.Fire(RegistrationAction.RollbackNotification));

            registrationStateMachine.Configure(RegistrationTransactionState.NotificationRollback)
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
            return registrationStateMachine.State == RegistrationTransactionState.ActivityCreated;
            
        }

    }
}
