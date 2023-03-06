namespace DuyProject.API.Templates
{
    public static class EmailTemplate
    {
        public struct TemplateWelcome
        {
            public const string Body = @"<p>Welcome,</p>
<p>You've been assigned to become an administrator for Kumpool Loyalty. Please click on the link to set your password <a href='{0}'>{0}</a> .<br />Your temporary password: {1}</p>
        <p>Thank you,<br />Kumpool Loyalty Admin.</p>";

            public const string Subject = "Welcome";
        }

        public struct TemplateForgotPassword
        {
            public const string Body = @"<p>Hi there,</p>
<p>You've requested to change or recover your password. Please click on the link to set your password <a href='{0}'>{0}</a>.<br />Your temporary password: {1}</p>
<p>If you don't request it, please ignore this email and contact your administrator immediately.</p>
<p>Reagards,<br />Kumpool Loyalty Admin.</p>";

            public const string Subject = "Forgot password";
        }

    }
}
