using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using ApeFree.AndroidPlus.Core.Lang;
using ApeFree.AndroidPlus.Core.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ApeFree.AndroidPlus.Dialog
{

    public class InputDialog : BaseDialog<EditText, string>                   //继承BaseDialog并指定泛型
    {
        public EditText EditText { get; }
        public Button OkButton { get; }
        public InputDialog(Context context) : base(context)
        {
            EditText = new EditText(context);
            ContentView = EditText;
            OkButton = AddButton("OK", (dialog, button) => { Result.Data = EditText.Text; Result.IsCancel = false; Dismiss(); });
        }
    }

    public class EditDialog : BaseDialog<LinearLayout, string>
    {
        public TextView ContentText { get; }
        public EditText EditText { get; }
        public Button OkButton { get; }
        public EditDialog(Context context) : base(context)
        {
            ContentView = (LinearLayout)Inflater.Inflate(Resource.Layout.dialog_edit_text, null);
            ContentText = ContentView.FindViewById<TextView>(Resource.Id.tv_dialog_content_view_edit_prompt);
            EditText = ContentView.FindViewById<EditText>(Resource.Id.et_dialog_content_view_edit_textview);
            OkButton = AddButton("OK", (dialog, button) =>
            {
                Result.Data = EditText.Text;
                Result.IsCancel = false; 
                Dismiss();
            });
        }
    }

    public class PromptDialog : BaseDialog<TextView, bool>                   //继承BaseDialog并指定泛型
    {
        public TextView PromptText { get; }
        public Button OkButton { get; }

        public PromptDialog(Context context) : base(context)
        {
            PromptText = (TextView)Inflater.Inflate(Resource.Layout.dialog_content_view_prompt, null);
            ContentView = PromptText;

            //添加按钮
            OkButton = AddButton("OK", (dialog, button) => { Result.Data = true; Result.IsCancel = false; Dismiss(); });
        }
    }

    public class MessageDialog : BaseDialog<TextView, bool>                   //继承BaseDialog并指定泛型
    {
        public TextView MessageText { get; }

        public MessageDialog(Context context) : base(context)
        {
            MessageText = (TextView)Inflater.Inflate(Resource.Layout.dialog_content_view_prompt, null);
            ContentView = MessageText;
        }
    }

    //多输入框的Dialog
    public class MultiInputBoxDialog : BaseDialog<LinearLayout, List<string>>                   //继承BaseDialog并指定泛型
    {
        public Button OkButton { get; }
        public LinearLayout EditTextGroup { get; }

        public MultiInputBoxDialog(Context context) : base(context)
        {
            //线性布局，往里面添加编辑框
            EditTextGroup = (LinearLayout)Inflater.Inflate(Resource.Layout.dialog_multiitems, null);
            ContentView = EditTextGroup;

            //添加按钮
            OkButton = AddButton("OK", (dialog, button) =>
            {
                List<string> values = new List<string>();
                for (int i = 0; i < EditTextGroup.ChildCount; i++)
                {
                    var et = (EditText)EditTextGroup.GetChildAt(i);
                    values.Add(et.Text);
                }
                Result.Data = values;
                Result.IsCancel = false;
                Dismiss();
            });
        }

        public EditText AddEditTextView(string hint = null)
        {
            //创建编辑框
            var et = new EditText(Context);
            //设置Hint属性
            et.Hint = hint;
            //将编辑框添加到布局中
            EditTextGroup.AddView(et);
            return et;
        }
    }

    //选择的对话框，列表形式
    public class SelectDialog<T> : BaseDialog<ListView, T>
    {
        //构造方法                  
        public SelectDialog(Context context, IEnumerable<T> data, Action<View, T> showHandler = null) : base(context)
        {
            //将ListView传给视图
            ContentView = new ListView(context);

            // 默认允许取消
            Cancelable = true;

            // 隐藏取消键
            CancelButton.Visibility = ViewStates.Gone;

            //创建适配器
            var adapter = new Adapter<Bean<T>, TextView>(Context, data.ToBeans(false), Resource.Layout.dialog_select_item);
            //对事件处理做出监听
            EventHandler clickEventHandler = (s, e) =>
            {
                TextView v = (TextView)s;
                Bean<T> itemData = (Bean<T>)v.Tag;

                Result.Data = itemData.Entity;
                Result.IsCancel = false;
                Dismiss();
            };
            adapter.LoadDataHandler = (itemData, v, pos) =>
            {
                v.Click -= clickEventHandler;
                v.Click += clickEventHandler;

                v.Tag = itemData;

                if (showHandler != null)
                {
                    showHandler?.Invoke(v, itemData.Entity);
                }
                else
                {
                    //为CheckBox设置值
                    v.Text = itemData.Entity.ToString();
                }
            };
            //将视图与适配器绑定
            ContentView.Adapter = adapter;
        }
    }

    //选择的对话框，列表形式
    public class MultiSelectDialog<T> : BaseDialog<ListView, List<T>>
    {
        public Button OkButton { get; }

        //构造方法                  
        public MultiSelectDialog(Context context, IEnumerable<T> data, Action<CheckBox, T> showHandler = null) : base(context)
        {
            //将ListView传给视图
            ContentView = new ListView(context);

            //创建适配器
            var adapter = new Adapter<Bean<T>, CheckBox>(Context, data.ToBeans(false), Resource.Layout.dialog_multiselect_item);
            //对事件处理做出监听
            EventHandler clickEventHandler = (s, e) =>
            {
                CheckBox v = (CheckBox)s;
                Bean<T> itemData = (Bean<T>)v.Tag;
                itemData.Tag = v.Checked;
                v.Tag = itemData;
            };
            adapter.LoadDataHandler = (itemData, v, pos) =>
            {
                v.Click -= clickEventHandler;
                v.Click += clickEventHandler;

                v.Tag = itemData;

                if (showHandler != null)
                {
                    showHandler?.Invoke(v, itemData.Entity);
                }
                else
                {
                    //为CheckBox设置值
                    v.Text = itemData.Entity.ToString();
                }
            };
            //将视图与适配器绑定
            ContentView.Adapter = adapter;

            //添加按钮                                      
            OkButton = AddButton("确定", (dialog, button) => { Result.Data = adapter.Data.Where(b => (bool)b.Tag == true).Select(b => b.Entity).ToList(); Result.IsCancel = false; Dismiss(); });
        }
    }

    //进度条的Dialog
    public class ProgressDialog : BaseDialog<LinearLayout, bool>                   //继承BaseDialog并指定泛型
    {
        //public LinearLayout LinearLayoutContent { get; }
        public ProgressBar ProgressBar { get; set; }
        public TextView TextView { get; set; }

        public ProgressDialog(Context context) : base(context)
        {
            ContentView = (LinearLayout)Inflater.Inflate(Resource.Layout.dialog_progress, null);
            //通过ContentView去获取布局里的两个控件
            ProgressBar = ContentView.FindViewById<ProgressBar>(Resource.Id.pb_dialog_content_view_progressbar);
            TextView = ContentView.FindViewById<TextView>(Resource.Id.tv_dialog_content_view_progressbar_content);
        }
    }

    //进度条的Dialog
    public class WaitingDialog : BaseDialog<LinearLayout, bool>                   //继承BaseDialog并指定泛型
    {
        //public LinearLayout LinearLayoutContent { get; }
        public ProgressBar ProgressBar { get; set; }
        public TextView TextView { get; set; }

        public WaitingDialog(Context context) : base(context)
        {
            ContentView = (LinearLayout)Inflater.Inflate(Resource.Layout.dialog_waiting, null);
            //通过ContentView去获取布局里的两个控件
            ProgressBar = ContentView.FindViewById<ProgressBar>(Resource.Id.pb_dialog_content_view_waiting);
            TextView = ContentView.FindViewById<TextView>(Resource.Id.tv_dialog_content_view_waiting_content);
        }

        public void Show(Action action)
        {
            Show();
            Task task = new Task(action);
            task.GetAwaiter().OnCompleted(() => Dismiss());
            task.Start();
        }

        public void Show(Task task)
        {
            if (task.IsCompleted) return;
            if (task.Status == TaskStatus.WaitingToRun) task.Start();
            Show(() => task.Wait());
        }

        public void Show(int milliseconds)
        {
            Show(() => Thread.Sleep(milliseconds));
        }

        //public void Show<T>(Task<T> task)
        //{
        //    Show();
        //    task.GetAwaiter().OnCompleted(() => Dismiss());
        //}
    }

    public class ViewPagerDialog : BaseDialog<ViewPager, bool>
    {
        public Button OkButton { get; }
        public Button BackButton { get; }
        public Button NextButton { get; }

        public ViewPagerDialog(Context context, IEnumerable<View> views = null) : base(context)
        {
            ContentView = new ViewPager(context);
            var adapter = new PagerAdapter();

            ContentView.Adapter = new PagerAdapter(views ?? new List<View>());

            //添加按钮
            OkButton = AddButton("OK", (dialog, button) => { Result.Data = true; Result.IsCancel = false; Dismiss(); });
            BackButton = AddButton("Back", (dialog, button) => ContentView.JumpToLast());
            NextButton = AddButton("Next", (dialog, button) => ContentView.JumpToNext());

            ContentView.PageSelected += ContentView_PageSelected;
        }

        private void ContentView_PageSelected(object sender, AndroidX.ViewPager.Widget.ViewPager.PageSelectedEventArgs e)
        {
            BackButton.Enabled = !ContentView.IsStartPage;
            NextButton.Enabled = !ContentView.IsLastPage;
        }

        public void Show(IEnumerable<View> views)
        {
            var adapter = (PagerAdapter)ContentView.Adapter;
            adapter.Views.Clear();
            adapter.Views.AddRange(views);
            adapter.NotifyDataSetChanged();
            Show();
        }
    }
}