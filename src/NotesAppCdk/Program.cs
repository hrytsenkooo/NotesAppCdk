using Amazon.CDK;

namespace NotesAppCdk
{
    /// <summary>
    /// Entry point for the AWS CDK application. This class defines and synthesizes the NotesAppStack 
    /// which contains the infrastructure resources for the NotesApp.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            var app = new App();
            new NotesAppStack(app, "NotesAppStack", new StackProps
            {
                Env = new Amazon.CDK.Environment
                {
                    Account = System.Environment.GetEnvironmentVariable("CDK_DEFAULT_ACCOUNT"),
                    Region = System.Environment.GetEnvironmentVariable("CDK_DEFAULT_REGION")
                }
            });
            app.Synth();
        }
    }
}
