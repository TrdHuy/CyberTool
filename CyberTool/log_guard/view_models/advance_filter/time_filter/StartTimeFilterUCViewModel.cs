using cyber_base.view_model;
using log_guard.view_models.watcher;

namespace log_guard.view_models.advance_filter.time_filter
{
    internal class StartTimeFilterUCViewModel : TimeFilterUCViewModel
    {
        public StartTimeFilterUCViewModel(BaseViewModel parent) : base(parent)
        {
        }

        public override bool Filter(object obj)
        {
            var data = obj as LWI_ParseableViewModel;
            if (!IsFilterEnable || FilterContent == "")
            {
                return true;
            }

            if(data != null)
            {
                return data.LogDateTime >= CurrentFilterTime;
            }
           
            
            return true;
        }
    }
}
