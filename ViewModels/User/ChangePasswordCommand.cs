﻿namespace DuyProject.API.ViewModels.User
{
    public class ChangePasswordCommand
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
