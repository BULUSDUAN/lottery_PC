using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Forms;
using System.Drawing;
using System.Reflection;


namespace RedisBusinessManager
{
    public static class Tools
    {

        


        #region 线程同步赋值

        private delegate void _buttonEnableChange(Button bt, bool enable);
        /// <summary>
        /// Button改变Enable
        /// </summary>
        public static void InvokeButtonEnableChange(Button bt, bool enable)
        {
            if (bt.InvokeRequired)
            {
                bt.Invoke(new _buttonEnableChange(InvokeButtonEnableChange), bt, enable);
            }
            else
            {
                bt.Enabled = enable;
            }
        }

        private delegate void _showLableDelegate(Label lb, string text);
        /// <summary>
        /// Lable显示文本
        /// </summary>
        public static void InvokeShowLableText(Label lb, string text)
        {
            if (lb.InvokeRequired)
            {
                lb.Invoke(new _showLableDelegate(InvokeShowLableText), lb, text);
            }
            else
            {
                lb.Text = text;
            }
        }

        private delegate void _setLableColorDelegate(Label lb, Color c);
        /// <summary>
        /// Lable文本颜色
        /// </summary>
        public static void InvokeSetLableColor(Label lb, Color c)
        {
            if (lb.InvokeRequired)
            {
                lb.Invoke(new _setLableColorDelegate(InvokeSetLableColor), lb, c);
            }
            else
            {
                lb.ForeColor = c;
            }
        }

        private delegate string _getLableDelegate(Label lb);
        /// <summary>
        /// Lable获取文本
        /// </summary>
        public static string InvokeGetLable(Label lb)
        {
            if (lb.InvokeRequired)
            {
                return lb.Invoke(new _getLableDelegate(InvokeGetLable), lb).ToString();
            }
            else
            {
                return lb.Text;
            }
        }

        private delegate void _showTextBoxDelegate(TextBox txt, string text);
        /// <summary>
        /// TextBox显示文本
        /// </summary>
        public static void InvokeShowTextBox(TextBox txt, string text)
        {
            if (txt.InvokeRequired)
            {
                txt.Invoke(new _showTextBoxDelegate(InvokeShowTextBox), txt, text);
            }
            else
            {
                txt.Text = text;
            }
        }

        private delegate void _addListBoxItemDelegate(ListBox list, object item);
        /// <summary>
        /// ListBox添加项
        /// </summary>
        public static void InvokeAddListBoxItem(ListBox list, object item)
        {
            if (list.InvokeRequired)
            {
                list.Invoke(new _addListBoxItemDelegate(InvokeAddListBoxItem), list, item);
            }
            else
            {
                list.Items.Add(item);
            }
        }
        private delegate void _addListViewItemDelegate(ListView list, ListViewItem item);
        /// <summary>
        /// ListView添加项
        /// </summary>
        public static void InvokeAddListViewItem(ListView list, ListViewItem item)
        {
            if (list.InvokeRequired)
            {
                list.Invoke(new _addListViewItemDelegate(InvokeAddListViewItem), list, item);
            }
            else
            {
                list.Items.Add(item);
            }
        }
        private delegate void _deleteListBoxItemDelegate(ListBox list, object item);
        /// <summary>
        /// ListBox删除项
        /// </summary>
        public static void InvokeDeleteListBoxItem(ListBox list, object item)
        {
            if (list.InvokeRequired)
            {
                list.Invoke(new _deleteListBoxItemDelegate(InvokeDeleteListBoxItem), list, item);
            }
            else
            {
                list.Items.Remove(item);
            }
        }

        private delegate void _selectListBoxItemDelegate(ListBox list, int index);
        /// <summary>
        /// ListBox选中项
        /// </summary>
        public static void InvokeSelectListBoxItem(ListBox list, int index)
        {
            if (list.InvokeRequired)
            {
                list.Invoke(new _selectListBoxItemDelegate(InvokeSelectListBoxItem), list, index);
            }
            else
            {
                list.SelectedIndex = index;
            }
        }

        private delegate int _getListBoxItemCount(ListBox list);
        /// <summary>
        /// ListBox获取项的条数
        /// </summary>
        public static int InvokeGetListBoxItemCount(ListBox list)
        {
            if (list.InvokeRequired)
            {
                return int.Parse(list.Invoke(new _getListBoxItemCount(InvokeGetListBoxItemCount), list).ToString());
            }
            else
            {
                return list.Items.Count;
            }
        }

        private delegate object _getListBoxItemDelegate(ListBox list, int index);
        /// <summary>
        /// ListBox获取项
        /// </summary>
        public static object InvokeGetListBoxItem(ListBox list, int index)
        {
            if (list.InvokeRequired)
            {
                return list.Invoke(new _getListBoxItemDelegate(InvokeGetListBoxItem), list, index);
            }
            else
            {
                return list.Items[index];
            }
        }

        private delegate object _getComboBoxItemDelegate(ComboBox list, int index);
        /// <summary>
        /// ComboBox获取项
        /// </summary>
        public static object InvokeGetComboBoxItem(ComboBox list, int index)
        {
            if (list.InvokeRequired)
            {
                return list.Invoke(new _getComboBoxItemDelegate(InvokeGetComboBoxItem), list, index);
            }
            else
            {
                return list.Items[index];
            }
        }

        private delegate void _clearListBoxItemDelegate(ListBox list);
        /// <summary>
        /// ListBox清空项
        /// </summary>
        public static void InvokeClearListBox(ListBox list)
        {
            if (list.InvokeRequired)
            {
                list.Invoke(new _clearListBoxItemDelegate(InvokeClearListBox), list);
            }
            else
            {
                list.Items.Clear();
            }
        }
        private delegate void _clearListViewItemDelegate(ListView list);
        /// <summary>
        /// ListView清空项
        /// </summary>
        public static void InvokeClearListView(ListView list)
        {
            if (list.InvokeRequired)
            {
                list.Invoke(new _clearListViewItemDelegate(InvokeClearListView), list);
            }
            else
            {
                list.Items.Clear();
            }
        }

        private delegate int _getListBoxSelectIndexDelegate(ListBox list);
        /// <summary>
        /// ListBox选中项的索引
        /// </summary>
        public static int InvokeGetListBoxSelectIndex(ListBox list)
        {
            if (list.InvokeRequired)
            {
                return int.Parse(list.Invoke(new _getListBoxSelectIndexDelegate(InvokeGetListBoxSelectIndex), list).ToString());
            }
            else
            {
                return list.SelectedIndex;
            }
        }

        private delegate int _getComboBoxSelectIndexDelegate(ComboBox list);
        /// <summary>
        /// ComboBox选中项的索引
        /// </summary>
        public static int InvokeGetComboBoxSelectIndex(ComboBox list)
        {
            if (list.InvokeRequired)
            {
                return int.Parse(list.Invoke(new _getComboBoxSelectIndexDelegate(InvokeGetComboBoxSelectIndex), list).ToString());
            }
            else
            {
                return list.SelectedIndex;
            }
        }

        private delegate int _getListBoxItemIndex(ListBox list, object item);
        /// <summary>
        /// 获取列表中项所在的索引
        /// </summary>
        public static int InvokeGetListBoxItemIndex(ListBox list, object item)
        {
            if (list.InvokeRequired)
            {
                return int.Parse(list.Invoke(new _getListBoxItemIndex(InvokeGetListBoxItemIndex), list, item).ToString());
            }
            else
            {
                return list.Items.IndexOf(item);
            }
        }

        private delegate void _setButtonEnable(Button bt, bool enable);
        /// <summary>
        /// 设置button状态
        /// </summary>
        public static void InvokeSetButtonEnable(Button bt, bool enabled)
        {
            if (bt.InvokeRequired)
            {
                bt.Invoke(new _setButtonEnable(InvokeSetButtonEnable), bt, enabled);
            }
            else
            {
                bt.Enabled = enabled;
            }
        }

        private delegate void _setLabelText(Label bt, string text);
        /// <summary>
        /// 设置Label状态
        /// </summary>
        public static void InvokeSetLabelText(Label bt, string text)
        {
            if (bt.InvokeRequired)
            {
                bt.Invoke(new _setLabelText(InvokeSetLabelText), bt, text);
            }
            else
            {
                bt.Text = text;
            }
        }

        private delegate void _setTextBoxText(TextBox bt, string text);
        /// <summary>
        /// 设置TextBox状态
        /// </summary>
        public static void InvokeSetTextBoxText(TextBox bt, string text)
        {
            if (bt.InvokeRequired)
            {
                bt.Invoke(new _setTextBoxText(InvokeSetTextBoxText), bt, text);
            }
            else
            {
                bt.Text = text;
            }
        }

        private delegate void _setRichTextBoxText(RichTextBox bt, string text);
        /// <summary>
        /// 设置TextBox状态
        /// </summary>
        public static void InvokeSetRichTextBoxText(RichTextBox bt, string text)
        {
            if (bt.InvokeRequired)
            {
                bt.Invoke(new _setRichTextBoxText(InvokeSetRichTextBoxText), bt, text);
            }
            else
            {
                bt.Text = text;
            }
        }

        private delegate void _setCheckBoxText(CheckBox bt, string text);
        /// <summary>
        /// 设置CheckBox状态
        /// </summary>
        public static void InvokeSetCheckBoxText(CheckBox bt, string text)
        {
            if (bt.InvokeRequired)
            {
                bt.Invoke(new _setCheckBoxText(InvokeSetCheckBoxText), bt, text);
            }
            else
            {
                bt.Text = text;
            }
        }

        private delegate void _setCheckBoxChecked(CheckBox bt, bool text);
        /// <summary>
        /// 设置CheckBox状态
        /// </summary>
        public static void InvokeSetCheckBoxChecked(CheckBox bt, bool text)
        {
            if (bt.InvokeRequired)
            {
                bt.Invoke(new _setCheckBoxChecked(InvokeSetCheckBoxChecked), bt, text);
            }
            else
            {
                bt.Checked = text;
            }
        }

        private delegate bool _getCheckBoxChecked(CheckBox bt);
        /// <summary>
        /// 设置CheckBox状态
        /// </summary>
        public static bool InvokeGetCheckBoxChecked(CheckBox bt)
        {
            if (bt.InvokeRequired)
            {
                return (bool)bt.Invoke(new _getCheckBoxChecked(InvokeGetCheckBoxChecked), bt);
            }
            else
            {
                return bt.Checked;
            }
        }

        #endregion

        /// <summary>
        /// 执行一个定时操作
        /// </summary>
        public static System.Timers.Timer ExcuteByTimer(double interval, Action callBack)
        {
            System.Timers.Timer timer = new System.Timers.Timer(interval);
            timer.AutoReset = false;
            timer.Enabled = true;
            timer.Elapsed += new System.Timers.ElapsedEventHandler((object sender, System.Timers.ElapsedEventArgs e) =>
            {
                timer.Close();
                timer.Dispose();

                //回调
                callBack();
            });
            return timer;
        }

        /// <summary>
        /// 查找所有的自动任务
        /// </summary>
        public static List<AutoTaskBase> FindAutoTaskList()
        {
            var result = new List<AutoTaskBase>();
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes().Where(t => t.GetInterfaces().Contains(typeof(IAutoTask))))
                .ToList();
            foreach (var t in types)
            {
                if (t.IsAbstract)
                    continue;
                result.Add(Activator.CreateInstance(t) as AutoTaskBase);
            }

            return result;
        }
    }
}
