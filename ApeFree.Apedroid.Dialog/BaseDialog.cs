using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ApeFree.Apedroid.Dialog
{

    /// <summary>
    /// 对话框标准接口
    /// </summary>
    public interface IDialog
    {
        void Show();
        void Dismiss();
        void Finish();
    }

    /// <summary>
    /// 基本对话框
    /// </summary>
    /// <typeparam name="TView">内部视图类型</typeparam>
    /// <typeparam name="TResult">返回数据类型</typeparam>
    public class BaseDialog<TView, TResult> : IDialog where TView : View
    {
        public delegate void DialogEventHandler(object sender, EventArgs e);
        public event DialogEventHandler Showing;
        public event DialogEventHandler Shown;
        public event DialogEventHandler Dismissing;
        public event DialogEventHandler Dismissed;

        public Context Context { get; }
        public AlertDialog InnerDialog { get; }
        public LayoutInflater Inflater { get; }

        public Result<TResult> Result { get; }

        /// <summary>
        /// 是否允许取消
        /// 默认不允许
        /// </summary>
        public bool Cancelable { get => cancelable; set { cancelable = value; InnerDialog.SetCancelable(cancelable); InnerDialog.SetCanceledOnTouchOutside(cancelable); } }
        protected bool cancelable;

        /// <summary>
        /// 标题栏图标控件
        /// </summary>
        public ImageView Icon { get; }
        /// <summary>
        /// 标题文本控件
        /// </summary>
        public TextView Title { get; }
        /// <summary>
        /// 标题栏
        /// </summary>
        public LinearLayout TitleBar { get; }
        /// <summary>
        /// 主视图容器
        /// </summary>
        protected LinearLayout ContentBox { get; }
        /// <summary>
        /// 按钮栏
        /// </summary>
        public LinearLayout ButtonBar { get; }
        /// <summary>
        /// 取消按钮
        /// </summary>
        public Button CancelButton { get; }

        //中间的内容部分
        public TView ContentView
        {
            get
            {
                if (ContentBox.ChildCount == 0) return default(TView);
                return (TView)ContentBox.GetChildAt(0);
            }
            set
            {
                ContentBox.RemoveAllViews();
                ContentBox.AddView(value);

            }
        }

        protected BaseDialog(Context context)
        {
            Context = context;

            Result = new Result<TResult>(context);

            Inflater = LayoutInflater.From(context);
            InnerDialog = new AlertDialog.Builder(context).Create();

            View view = Inflater.Inflate(Resource.Layout.dialog_base, null);
            TitleBar = view.FindViewById<LinearLayout>(Resource.Id.ly_dialog_title_bar);
            Icon = view.FindViewById<ImageView>(Resource.Id.iv_dialog_title_img);
            Title = view.FindViewById<TextView>(Resource.Id.tv_dialog_title_text);
            ContentBox = view.FindViewById<LinearLayout>(Resource.Id.ly_dialog_content);
            ButtonBar = view.FindViewById<LinearLayout>(Resource.Id.ly_dialog_button_group);
            CancelButton = view.FindViewById<Button>(Resource.Id.btn_dialog_cancel);

            CancelButton.Click += CancelButton_Click;

            // 默认不允许取消（点击对话框外的方式区域取消）
            Cancelable = false;

            Icon.SetImageDrawable(GetDefaultIcon());

            // 封装Android原生AlertDialog对象
            InnerDialog.SetView(view);
            InnerDialog.CancelEvent += InnerDialog_CancelEvent;
        }

        //点击事件
        private void InnerDialog_CancelEvent(object sender, EventArgs e)
        {
            Result.IsCancel = true;
        }

        //取消按钮的单击事件
        private void CancelButton_Click(object sender, EventArgs e)
        {
            Result.IsCancel = true;
            Dismiss();
        }

        //构造方法
        public BaseDialog(Context context, TView contentView) : this(context)                       
        {
            ContentView = contentView;
        }

        

        /// <summary>
        /// 添加按钮到按钮栏中
        /// </summary>
        /// <param name="button"></param>
        public void AddButton(Button button)
        {
            // 新增Button的布局参数与[取消按钮]的布局一致
            button.LayoutParameters = CancelButton.LayoutParameters;

            ButtonBar.AddView(button);
        }

        /// <summary>
        /// 动态创建拥有回调事件的按钮并添加到按钮栏中
        /// </summary>
        /// <param name="buttonName"></param>
        /// <param name="clickCallback"></param>
        /// <returns></returns>
        public Button AddButton(string buttonName, Action<BaseDialog<TView, TResult>, Button> clickCallback)
        {
            // 创建按钮
            Button button = new Button(Context) { Text = buttonName };

            // 按钮的单击事件
            button.Click += (s, e) => clickCallback?.Invoke(this, button);

            // 将新按钮添加入按钮栏中
            AddButton(button);

            // 返回新创建的按钮对象
            return button;
        }

        /// <summary>
        /// 显示对话框
        /// </summary>
        public void Show()
        {
            Showing?.Invoke(this, new EventArgs());

            // 如果没有可现实的按钮，则让按钮容器隐藏
            if(ButtonBar.GetViews().FirstOrDefault(v=>v.Visibility==ViewStates.Visible) != null)
            {
                ButtonBar.Visibility = ViewStates.Visible;
            }
            else
            {
                ButtonBar.Visibility = ViewStates.Gone;
            }

            InnerDialog.ShowEvent+=(s,e)=> Shown?.Invoke(this, new EventArgs());
            InnerDialog.Show();

        }

        /// <summary>
        /// 销毁对话框
        /// </summary>
        public void Dismiss()
        {
            Dismissing?.Invoke(this, new EventArgs());
            InnerDialog.Dismiss();
            Dismissed?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// 结束页面
        /// </summary>
        public void Finish()
        {
            Dismissing?.Invoke(this, new EventArgs());
            Finish();
            Dismissed?.Invoke(this, new EventArgs());
        }

        //默认的图标
        private Drawable GetDefaultIcon()
        {
            ApplicationInfo applicationInfo;
            PackageManager packageManager;
            try
            {
                packageManager = Context.ApplicationContext.PackageManager;
                applicationInfo = packageManager.GetApplicationInfo(Context.PackageName, 0);
            }
            catch (PackageManager.NameNotFoundException e)
            {
                throw e;
            }
            Drawable d = packageManager.GetApplicationIcon(applicationInfo); //xxx根据自己的情况获取drawable
            return d;
        }
    }


    /// <summary>
    /// 对话框返回结果
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Result<T>
    {
        public Context Context { get; }

        public T Data { get; set; }
        public bool IsCancel { get; set; }

        public Result(Context context)
        {
            IsCancel = true;
            Context = context;
        }
    }

}