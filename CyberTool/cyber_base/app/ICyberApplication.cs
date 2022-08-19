using cyber_base.async_task;
using cyber_base.definition;
using cyber_base.implement.async_task;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace cyber_base.app
{
    public interface ICyberApplication
    {
        /// <summary>
        /// App instance lúc runtime 
        /// </summary>
        Application CyberApp { get; }

        /// <summary>
        /// Mở cửa sổ thực hiện 1 task không đồng bộ
        /// </summary>
        /// <param name="content"></param>
        /// <param name="title"></param>
        /// <param name="asyncTask"></param>
        /// <param name="canExecute"></param>
        /// <param name="callback"></param>
        /// <param name="delayTime"></param>
        /// <param name="estimatedTime"></param>
        /// <param name="taskName"></param>
        /// <returns></returns>
        CyberContactMessage OpenWaitingTaskBox(string content
            , string title
            , Func<object, AsyncTaskResult, CancellationTokenSource, Task<AsyncTaskResult>> asyncTask
            , Func<object, bool>? canExecute = null
            , Func<object, AsyncTaskResult, Task<AsyncTaskResult>>? callback = null
            , ulong delayTime = 0
            , ulong estimatedTime = 0
            , string taskName = "");

        /// <summary>
        /// Mở cửa sổ thực hiện 1 hoặc nhiều task không đồng bộ
        /// </summary>
        /// <param name="title"></param>
        /// <param name="task"></param>
        /// <param name="isCancelable"></param>
        /// <param name="multiTaskDoneCallback"></param>
        /// <param name="isUseMultiTaskReport"></param>
        /// <returns></returns>
        CyberContactMessage OpenMultiTaskBox(string title
            , MultiAsyncTask task
            , bool isCancelable = true
            , Action<object>? multiTaskDoneCallback = null
            , bool isUseMultiTaskReport = true);

        /// <summary>
        /// Mở cửa sổ hiển thị cảnh báo
        /// </summary>
        /// <param name="warning"></param>
        /// <param name="isDialog"></param>
        /// <returns></returns>
        CyberContactMessage ShowWaringBox(string warning
            , bool isDialog = true);

        /// <summary>
        /// Mở cửa sổ hiển thị câu hỏi có/không
        /// Trả về giá trị có hoặc không
        /// </summary>
        /// <param name="question"></param>
        /// <param name="isDialog"></param>
        /// <returns></returns>
        CyberContactMessage ShowYesNoQuestionBox(string question
            , bool isDialog = true);

        /// <summary>
        /// Mở cửa sổ chọn file và trả về đường dẫn đến file đó
        /// </summary>
        /// <param name="title">Tiêu đề của cửa sổ</param>
        /// <param name="filter">Định dạng file cần lọc</param>
        /// <returns></returns>
        string OpenFileChooserDialogWindow(string title = "Choose a log file"
            , string filter = "Text files (*.txt)|*.txt|Log files (*.log)|*.log");

        /// <summary>
        /// Mở cửa sổ mới có chứa 1 đối tượng view
        /// Đối tượng view này là content của ContentControl (cc)
        /// </summary>
        /// <param name="cc">Control chứa nội dung cần trình chiếu</param>
        /// <param name="opener">Đối tượng request mở window</param>
        /// <param name="ownerWindow">Đối tượng chứa window</param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="dataContext"></param>
        /// <param name="windowShowedCallback"></param>
        /// <param name="title"></param>
        void ShowPopupCControl(ContentControl cc
           , UIElement opener
           , CyberOwner ownerWindow = CyberOwner.Default
           , double width = 500
           , double height = 400
           , object? dataContext = null
           , Action<object>? windowShowedCallback = null
           , string title = "Floating window");

        /// <summary>
        /// Mở cửa sổ mới có chứa 1 đối tượng usercontrol
        /// </summary>
        /// <param name="uc"></param>
        /// <param name="opener"></param>
        /// <param name="ownerWindow"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="dataContext"></param>
        /// <param name="windowShowedCallback"></param>
        /// <param name="title"></param>
        void ShowUserControlWindow(UserControl uc
            , CyberOwner ownerWindow = CyberOwner.Default
            , double width = 500
            , double height = 400
            , Action<object>? windowShowedCallback = null
            , string title = "Floating window"
            );

        /// <summary>
        /// Mở cửa sổ chọn folder và trả về đường dẫn tới folder
        /// đó
        /// </summary>
        /// <returns></returns>
        string OpenFolderChooserDialogWindow();

        /// <summary>
        /// Mở cửa sổ edit text, với oldText là chuỗi cần edit
        /// và trả về chuỗi mới
        /// </summary>
        /// <param name="oldText"></param>
        /// <param name="isMultiLine">bật chế độ multi line cho edit text</param>
        /// <returns></returns>
        string OpenEditTextDialogWindow(string oldText, bool isMultiLine = false);
    }
}
