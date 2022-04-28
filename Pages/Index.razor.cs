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


        async Task OnFinish(EditContext editContext)
        {
            uploadStepCurrent = 1;
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
