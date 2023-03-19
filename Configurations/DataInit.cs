//using DuyProject.API.Services;
//using DuyProject.API.ViewModels;
//using DuyProject.API.ViewModels.User;

//namespace DuyProject.API.Configurations;

//public static class DataInit
//{
//    public static async Task InitializeData(WebApplication app)
//    {
//        //var users = new List<UserCreateCommand>
//        //{
//        //    new UserCreateCommand
//        //    {
//        //        Name = "Administrator",
//        //        Password = "Abc@123456",
//        //        Email = "admin@abc",
//        //        Role = AppSettings.AdminRole
//        //    },
//        //    new UserCreateCommand
//        //    {
//        //        Name = "Test User",
//        //        Password = "Abc@123456",
//        //        Email = "test@abc",
//        //        Role = AppSettings.Patient
//        //    },
//        //    new UserCreateCommand
//        //    {
//        //        Name = "Test User",
//        //        Password = "Abc@123456",
//        //        Email = "benh@abc",
//        //        Role = AppSettings.AdminRole
//        //    }
//        //};

//        //var userService = app.Services.GetService<UserService>();
//        //if (userService != null)
//        //{
//        //    foreach (UserCreateCommand user in users)
//        //    {
//        //        ServiceResult<UserViewModel> userExists = await userService.GetByUserNameAsync(user.Name);
//        //        if (userExists.Success) break;
//        //        await userService.Create(user);
//        //    }
//        //}
//    }
//}