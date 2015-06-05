using System.Windows.Forms;

namespace TogglDesktop
{
    class ComboBoxHelper
    {
        public static ulong SelectedItemID(ComboBox combobox)
        {
            for (int i = 0; i < combobox.Items.Count; i++)
            {
                Toggl.Model item = (Toggl.Model)combobox.Items[i];
                if (item.Name == combobox.Text)
                {
                    return item.ID;
                }
            }
            return 0;
        }

        public static ulong SelectedWorkspaceItemID(ComboBox combobox)
        {
            for (int i = 0; i < combobox.Items.Count; i++)
            {
                Toggl.Workspace item = (Toggl.Workspace)combobox.Items[i];
                if (item.Name == combobox.Text)
                {
                    return item.ID;
                }
            }
            return 0;
        }
    }
}
