using Dislinkt.Saga.Data;
using Dislinkt.Saga.Proxy;

namespace Dislinkt.Saga.Menager
{
    public class RegistrationMenager:IRegistrationMenager
    {
        private readonly IProfileProxy profileProxy;
        private readonly IConnectionProxy connectionProxy;
        private readonly IJobsProxy jobsProxy;
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
            ConnectionRollback,
            JobsUserCreated,
            JobsUserRollback,
            JobsUserFailed,
            NotificationCreated,
            NotificationCareateFailed,
            NotificationRollback,
            ActivityCreated,
            ActivityCreatedFailed,
            ActivityRollback
        }

        enum RegistrationAction
        {
            CreateProfile,
            RollbackProfile,
            CancelProfile,
            CreateConnection,
            RollbackConnection,
            CreateJobsUser,
            RollbackJobsUser,
            CreateNotification,
            RollbackNotification,
            CreateActivity,
            RollbackActivity
        }

        public RegistrationMenager(IProfileProxy profileProxy,IConnectionProxy connectionProxy,IJobsProxy jobsProxy, INotificationProxy notificationProxy,IActivityProxy activityProxy)
        {
            this.profileProxy = profileProxy;
            this.notificationProxy= notificationProxy;
            this.connectionProxy= connectionProxy;
            this.jobsProxy = jobsProxy;
            this.activityProxy = activityProxy;
        }

        public bool Register(UserData input)
        {
            var registrationStateMachine = new Stateless.StateMachine<RegistrationTransactionState, RegistrationAction>(
                RegistrationTransactionState.NotStarted);

            var user = new User();
            var createdUser = new User();
            var isSuccess = false;

            registrationStateMachine.Configure(RegistrationTransactionState.NotStarted)
                .PermitDynamic(RegistrationAction.CreateProfile, () => {
                    (user, isSuccess) = profileProxy.CreateUser(input).Result;
                    return isSuccess ? RegistrationTransactionState.ProfileCreated : RegistrationTransactionState.ProfileCreateFailed;
                });


            registrationStateMachine.Configure(RegistrationTransactionState.ProfileCreated)
                .PermitDynamic(RegistrationAction.CreateConnection, () => {
                    (createdUser, isSuccess) = connectionProxy.CreateNode(user).Result;
                    return isSuccess ? RegistrationTransactionState.ConnectionCreated : RegistrationTransactionState.ConnectionCreatedFailed;
                })
                .OnEntry(()=>registrationStateMachine.Fire(RegistrationAction.CreateConnection));


            registrationStateMachine.Configure(RegistrationTransactionState.ConnectionCreated)
                .PermitDynamic(RegistrationAction.CreateJobsUser, () => {
                    (createdUser, isSuccess) = jobsProxy.CreateNode(user).Result;
                    return isSuccess ? RegistrationTransactionState.JobsUserCreated : RegistrationTransactionState.JobsUserFailed;
                })
                .OnEntry(() => registrationStateMachine.Fire(RegistrationAction.CreateJobsUser));


            registrationStateMachine.Configure(RegistrationTransactionState.JobsUserCreated)
                .PermitDynamic(RegistrationAction.CreateNotification, () => {
                    (createdUser, isSuccess) = notificationProxy.CreateNotificationSetting(user).Result;
                    return isSuccess ? RegistrationTransactionState.NotificationCreated : RegistrationTransactionState.NotificationCareateFailed;
                })
                .OnEntry(() => registrationStateMachine.Fire(RegistrationAction.CreateNotification));


            registrationStateMachine.Configure(RegistrationTransactionState.NotificationCreated)
                .PermitDynamic(RegistrationAction.CreateActivity, () => {
                    (createdUser, isSuccess) = activityProxy.CreateActivity(user).Result;
                    
                    return isSuccess ? RegistrationTransactionState.ActivityCreated : RegistrationTransactionState.ActivityCreatedFailed;
                })
                .OnEntry(() => registrationStateMachine.Fire(RegistrationAction.CreateActivity));


            //rolback

            registrationStateMachine.Configure(RegistrationTransactionState.ActivityCreatedFailed)
                .PermitDynamic(RegistrationAction.RollbackActivity, () =>{
                  // notificationProxy.DeleteNotificationAsync(user);
                    return RegistrationTransactionState.ActivityRollback;
                })
                .OnEntry(() => registrationStateMachine.Fire(RegistrationAction.RollbackActivity));


            registrationStateMachine.Configure(RegistrationTransactionState.NotificationCareateFailed)
                .PermitDynamic(RegistrationAction.RollbackNotification, () =>{
                   // notificationProxy.DeleteNotificationAsync(user);
                    return RegistrationTransactionState.NotificationRollback;
                })
                .OnEntry(() => registrationStateMachine.Fire(RegistrationAction.RollbackNotification));


            registrationStateMachine.Configure(RegistrationTransactionState.JobsUserFailed)
                .PermitDynamic(RegistrationAction.RollbackJobsUser, () =>{
              // notificationProxy.DeleteNotificationAsync(user);
                    return RegistrationTransactionState.JobsUserRollback;
                })
                .OnEntry(() => registrationStateMachine.Fire(RegistrationAction.RollbackJobsUser));

            registrationStateMachine.Configure(RegistrationTransactionState.ConnectionCreatedFailed)
                .PermitDynamic(RegistrationAction.RollbackConnection, () => {
                // notificationProxy.DeleteNotificationAsync(user);
                    return RegistrationTransactionState.ConnectionRollback;
                })
                .OnEntry(() => registrationStateMachine.Fire(RegistrationAction.RollbackConnection));


            //nastavak na rollback poruku

            registrationStateMachine.Configure(RegistrationTransactionState.ActivityRollback)
                .PermitDynamic(RegistrationAction.RollbackNotification, () =>{
                    notificationProxy.DeleteNotificationAsync(user);
                    return RegistrationTransactionState.NotificationRollback;
                })
                .OnEntry(() => registrationStateMachine.Fire(RegistrationAction.RollbackNotification));


            registrationStateMachine.Configure(RegistrationTransactionState.NotificationRollback)
                .PermitDynamic(RegistrationAction.RollbackJobsUser, () =>{
                    jobsProxy.DeleteNodeAsync(user);
                    return RegistrationTransactionState.JobsUserRollback;
                })
                .OnEntry(() => registrationStateMachine.Fire(RegistrationAction.RollbackJobsUser));


            registrationStateMachine.Configure(RegistrationTransactionState.JobsUserRollback)
                .PermitDynamic(RegistrationAction.RollbackConnection, () => {
                    connectionProxy.DeleteNodeAsync(user);
                    return RegistrationTransactionState.ConnectionRollback;
                })
                .OnEntry(() => registrationStateMachine.Fire(RegistrationAction.RollbackConnection));


            registrationStateMachine.Configure(RegistrationTransactionState.ConnectionRollback)
                .PermitDynamic(RegistrationAction.CancelProfile, () =>{
                    profileProxy.DeleteProfileAsync(user);
                    return RegistrationTransactionState.ProfileCancelled;
                })
                .OnEntry(() => registrationStateMachine.Fire(RegistrationAction.CancelProfile));



            registrationStateMachine.Fire(RegistrationAction.CreateProfile);
            return registrationStateMachine.State == RegistrationTransactionState.ActivityCreated;
            
        }

    }
}
