namespace Legerity.PageGenerator
{
    using System;
    using System.ComponentModel.Design;
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using EnvDTE;
    using EnvDTE80;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Interop;
    using Task = System.Threading.Tasks.Task;

    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class XamlPageGenerator
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0100;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("bd79e4b3-2b8e-4b9f-91d2-842078026838");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly AsyncPackage package;

        private DTE2 ideService;

        /// <summary>
        /// Initializes a new instance of the <see cref="XamlPageGenerator"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="commandService">Command service to add command to, not null.</param>
        private XamlPageGenerator(AsyncPackage package, OleMenuCommandService commandService)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new OleMenuCommand(this.Execute, menuCommandID);
            menuItem.BeforeQueryStatus += this.OnBeforeQueryStatus;
            commandService.AddCommand(menuItem);
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static XamlPageGenerator Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private Microsoft.VisualStudio.Shell.IAsyncServiceProvider ServiceProvider => this.package;

        private DTE2 IdeService => this.ideService ??=
            this.ServiceProvider.GetServiceAsync(typeof(SDTE)).GetAwaiter().GetResult() as DTE2;

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static async Task InitializeAsync(AsyncPackage package)
        {
            // Switch to the main thread - the call to AddCommand in XamlPageGenerator's constructor requires
            // the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            OleMenuCommandService commandService =
                await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new XamlPageGenerator(package, commandService);
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void Execute(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            ProjectItem xamlProjectItem = this.GetSelectedXamlProjectItem();
            if (xamlProjectItem != null)
            {
                string filePath = xamlProjectItem.Document.FullName;
            }
        }

        private void OnBeforeQueryStatus(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (!(sender is OleMenuCommand command))
            {
                return;
            }

            // Assume the menu item should not be shown.
            command.Visible = command.Enabled = false;

            // Update if XAML item selected.
            ProjectItem xamlProjectItem = this.GetSelectedXamlProjectItem();
            command.Visible = xamlProjectItem != null ? command.Enabled = true : command.Enabled = false;
        }

        private ProjectItem GetSelectedXamlProjectItem()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            ProjectItem selectedItem = this.GetSelectedProjectItem();
            return selectedItem.Name.EndsWith(".xaml", StringComparison.CurrentCultureIgnoreCase) ? selectedItem : null;
        }

        private ProjectItem GetSelectedProjectItem()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            object[] selectedItems = (object[])this.IdeService.ToolWindows.SolutionExplorer.SelectedItems;
            ProjectItem selectedProjectItem = selectedItems.FirstOrDefault() is UIHierarchyItem selectedItem
                ? selectedItem.Object as ProjectItem
                : null;
            return selectedProjectItem;
        }
    }
}