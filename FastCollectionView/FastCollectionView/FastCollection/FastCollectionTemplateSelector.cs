using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace Binwell.Controls.FastCollectionView.FastCollection
{
    public class FastCollectionTemplateSelector
    {
        public List<FastCollectionDataTemplate> DataTemplates { get; } = new List<FastCollectionDataTemplate>();

        protected readonly Dictionary<int, string> DataTemplateViewTypes = new Dictionary<int, string>();

        public DataTemplate SelectTemplate(object item)
        {
            return OnSelectTemplate(item);
        }

		public string GetKey(object item)
		{
			var template = OnSelectTemplate(item) as FastCollectionDataTemplate;
			return template?.Key;
		}

        public virtual DataTemplate OnSelectTemplate(object item) {
	        if (item == null) return null;
            var key = item.GetType().Name;
            return DataTemplates.FirstOrDefault(dt => dt.Key == key);
        }
        
	    public FastCollectionTemplateSelector Add(params FastCollectionDataTemplate[] dataTemplates) {
	        foreach (var dataTemplate in dataTemplates)
	        {
	            DataTemplates.Add(dataTemplate);
            }
		    return this;
	    }

        public FastCollectionTemplateSelector Prepare()
        {
            DataTemplateViewTypes.Clear();
            var id = 0;
            foreach (var dataTemplate in DataTemplates)
            {
                var key = dataTemplate.Key;
                DataTemplateViewTypes.Add(id++,key);
            }
	        return this;
        }

        public virtual int GetViewType(object item, BindableObject container)
        {
            var key = item.GetType().Name;
            return DataTemplateViewTypes.FirstOrDefault(dt => dt.Value == key).Key;
        }

        public DataTemplate OnSelectTemplateByViewType(int type, BindableObject container)
        {
            if (!DataTemplateViewTypes.ContainsKey(type)) return null;
            var key = DataTemplateViewTypes[type];
            return DataTemplates.FirstOrDefault(dt => dt.Key == key);
        }

        public Dictionary<int, Size> GetSizesByViewType()
        {
            return DataTemplateViewTypes.ToDictionary(t => t.Key, t=> DataTemplates.First(dt => dt.Key == t.Value).CellSize);
        }

        public Size GetSizesByKey(string key)
        {
	        var size = DataTemplates?.FirstOrDefault(dt => dt.Key == key)?.CellSize ?? Size.Zero;
	        if (double.IsNaN(size.Width)) size.Width = 0;
	        if (double.IsNaN(size.Height)) size.Height = 0;
	        return size;
        }
	}
}
