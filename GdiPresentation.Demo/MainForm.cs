using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace GdiPresentation.Demo
{
    public partial class MainForm : Form
    {
        private Demo _demo;

        public MainForm()
        {
            InitializeComponent();

            ElementStatistics.IsEnabled = true;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            foreach (var type in GetType().Assembly.GetTypes())
            {
                if (
                    typeof(Demo).IsAssignableFrom(type) &&
                    TypeUtil.CanBeInstantiated(type)
                ) {
                    var button = new ToolStripButton
                    {
                        Tag = type,
                        DisplayStyle = ToolStripItemDisplayStyle.Text,
                        Text = ReflectionExtensions.GetCustomAttribute<DisplayNameAttribute>(type).DisplayName,
                        CheckOnClick = true
                    };

                    _toolStrip.Items.Add(button);

                    button.CheckedChanged += button_CheckedChanged;
                }
            }

            using (var key = Program.BaseKey)
            {
                string previousType = key.GetValue("Demo") as string;

                if (previousType != null)
                {
                    var type = GetType().Assembly.GetType(previousType);

                    foreach (ToolStripButton button in _toolStrip.Items)
                    {
                        if (button.Tag as Type == type)
                        {
                            button.Checked = true;
                            break;
                        }
                    }
                }
            }
        }

        void button_CheckedChanged(object sender, EventArgs e)
        {
            _listBox.Items.Clear();
            ElementStatistics.GetNewEvents();

            var button = (ToolStripButton)sender;

            if (!button.Checked)
                return;

            foreach (ToolStripButton other in _toolStrip.Items)
            {
                if (other != button && other.Checked)
                    other.Checked = false;
            }

            if (_demo != null)
                _demo.Dispose();

            var type = (Type)((ToolStripButton)sender).Tag;

            using (var key = Program.BaseKey)
            {
                key.SetValue("Demo", type.FullName);
            }

            _demo = (Demo)Activator.CreateInstance(type);

            var control = _demo.CreateControl();

            control.Dock = DockStyle.Fill;

            _clientArea.Controls.Clear();
            _clientArea.Controls.Add(control);
        }

        private void _statisticsTimer_Tick(object sender, EventArgs e)
        {
            var events = ElementStatistics.GetNewEvents();

            if (events.Length == 0)
                return;

            _listBox.BeginUpdate();

            foreach (var @event in events)
            {
                var type = @event.Type;
                string typeName;

                if ((type & ElementStatisticsEventType.Forced) != 0)
                {
                    type &= ~ElementStatisticsEventType.Forced;
                    typeName = type.ToString() + " (F)";
                }
                else
                {
                    typeName = type.ToString();
                }

                string content = typeName + " " + @event.Duration.TotalMilliseconds + " ms";

                _listBox.Items.Add(content);
            }

            _listBox.SelectedIndex = _listBox.Items.Count - 1;

            _listBox.EndUpdate();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            _listBox.Items.Clear();
        }
    }
}
