using AntDesign;
using LANMovie.Common;
using LANMovie.Data.Entities;
using Microsoft.AspNetCore.Components.Forms;

namespace LANMovie.Pages
{
    partial class Index
    {
        bool emptyShown = true;

        int uploadStepCurrent = 0;
        VideoCategory uploadVideoCategory;
        
        MovieEntity movie = new();
        DateTime publishDate = DateTime.Now;


        /// <summary>
        /// 打开视频上传步骤条
        /// </summary>
        void OpenUploadStep()
        {
            emptyShown = false;
        }


        /// <summary>
        /// 视频上传步骤跳转
        /// </summary>
        /// <param name="current">待跳转到的步骤(起始步骤为0)</param>
        void ChangeStep(int current)
        {
            uploadStepCurrent = current;
        }


        async Task OnFinish(EditContext editContext)
        {

        }


        async Task OnFinishFailed(EditContext editContext)
        {

        }
    }

    enum VideoCategory
    {
        Movie = 0,
        Teleplay,
        ShortVideo,
        None
    }
}
