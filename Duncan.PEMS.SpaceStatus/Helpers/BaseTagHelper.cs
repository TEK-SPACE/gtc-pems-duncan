using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

using System.Web.Mvc;


using Duncan.PEMS.SpaceStatus.DataSuppliers;
using Duncan.PEMS.SpaceStatus.Models;

namespace Duncan.PEMS.SpaceStatus.Helpers
{
    public class BaseTagHelper
    {
        private TagBuilder _tagBuilder = null;

        public List<BaseTagHelper> Children = new List<BaseTagHelper>();

        public BaseTagHelper(string tagName)
        {
            _tagBuilder = new TagBuilder(tagName);
        }

        public IDictionary<string, string> Attributes
        {
            get { return this._tagBuilder.Attributes; }
        }

        public string IdAttributeDotReplacement
        {
            get { return this._tagBuilder.IdAttributeDotReplacement; }
            set { this._tagBuilder.IdAttributeDotReplacement = value; }
        }

        // We don't want to expose this, because we are using the Children collection to generate the underlying InnerHtml
        // However we will also include a "SetInnerHtml" method when its absolutely necessary to control it manually
        /*
        public string InnerHtml
        {
            get { return this.TagBuild.InnerHtml; }
            set { this.TagBuild.InnerHtml = value; }
        }
        */

        public string TagName
        {
            get { return this._tagBuilder.TagName; }
        }

        public void AddCssClass(string value)
        {
            this._tagBuilder.AddCssClass(value);
        }

        public void GenerateId(string name)
        {
            this._tagBuilder.GenerateId(name);
        }

        public void MergeAttribute(string key, string value)
        {
            this._tagBuilder.MergeAttribute(key, value);
        }

        public void MergeAttribute(string key, string value, bool replaceExisting)
        {
            this._tagBuilder.MergeAttribute(key, value, replaceExisting);
        }

        public void MergeAttributes<TKey, TValue>(IDictionary<TKey, TValue> attributes)
        {
            this._tagBuilder.MergeAttributes(attributes);
        }

        public void MergeAttributes<TKey, TValue>(IDictionary<TKey, TValue> attributes, bool replaceExisting)
        {
            this._tagBuilder.MergeAttributes(attributes, replaceExisting);
        }

        public void SetInnerText(string innerText)
        {
            this._tagBuilder.SetInnerText(innerText);
        }

        public void SetInnerHtml(string innerHtml)
        {
            this._tagBuilder.InnerHtml = innerHtml;
        }

        public string ToString(TagRenderMode renderMode)
        {
            // If there are any children, we will recursivley render
            // all of them to a stringbuilder, and then put the final 
            // result into our tagbuilder's InnerHTML and produce final 
            // rendering
            if ((Children != null) && (Children.Count > 0))
            {
                StringBuilder sb = new StringBuilder();
                foreach (BaseTagHelper nextChild in Children)
                {
                    sb.AppendLine(nextChild.ToString());
                }
                this._tagBuilder.InnerHtml = sb.ToString();
            }

            return this._tagBuilder.ToString(renderMode);
        }

        public override string ToString()
        {
            // If there are any children, we will recursivley render
            // all of them to a stringbuilder, and then put the final 
            // result into our tagbuilder's InnerHTML and produce final 
            // rendering
            if ((Children != null) && (Children.Count > 0))
            {
                StringBuilder sb = new StringBuilder();
                foreach (BaseTagHelper nextChild in Children)
                {
                    sb.AppendLine(nextChild.ToString());
                }
                this._tagBuilder.InnerHtml = sb.ToString();
            }

            return this._tagBuilder.ToString(TagRenderMode.Normal);
        }

    }
}