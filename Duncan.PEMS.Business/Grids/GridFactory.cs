using System.Collections.Generic;
using System.Linq;
using Duncan.PEMS.Business.Resources;
using Duncan.PEMS.Entities.Grids;
using Duncan.PEMS.Utilities;

namespace Duncan.PEMS.Business.Grids
{
    /// <summary>
    /// The <see cref="Duncan.PEMS.Business.Grids"/> namespace contains classes for managing the grids used throughout PEMS.
    /// </summary>
    [System.Runtime.CompilerServices.CompilerGenerated]
    class NamespaceDoc
    {
    }


    public class GridFactory : RbacBaseFactory
    {
        /// <summary>
        /// Gets any  CustomerGrid data for the customer, action and controller. The data is then localized for the current culture and returned.
        /// </summary>
        public  List<GridData> GetGridData(string controller,string action, int customerId)
        {
            //do not include the default role group (in constants class)
            var items = RbacEntities.CustomerGrids.Where(
                x => x.CustomerId == customerId && x.Controller == controller && x.Action == action).OrderBy(x=>x.OriginalPosition).Select(
                    x =>
                    new GridData
                        {
                            Title = x.Title,
                            Position = x.Position,
                            OriginalPosition =  x.OriginalPosition,
                            OriginalTitle = x.OriginalTitle,
                            IsHidden = x.IsHidden
                        }).ToList();
            //update the title to be the localized Term
            items.ForEach(r => UpdateColumnName(r));
            return items;
        }

        /// <summary>
        /// Gets customergrid data for the customer, action, and ctonroller where the data is NOT hidden. This is used for exporting. The data is then localized for the current culture and returned.
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        /// <param name="customerId"></param>
        /// <returns></returns>
        public List<GridData> GetExportGridData(string controller, string action, int customerId)
        {
            //do not include the default role group (in constants class)
            //since this is export, need ot order by new position and not include hidden items
            var items = RbacEntities.CustomerGrids.Where(
                x => x.CustomerId == customerId && x.Controller == controller && x.Action == action && !x.IsHidden).OrderBy(x => x.Position).Select(
                    x =>
                    new GridData
                    {
                        Title = x.Title,
                        Position = x.Position,
                        OriginalPosition = x.OriginalPosition,
                        OriginalTitle = x.OriginalTitle,
                        IsHidden = x.IsHidden
                    }).ToList();
            //update the title to be the localized Term
            items.ForEach(r => UpdateColumnName(r));
            return items;
        }

        /// <summary>
        /// Get the presently active grid data.  This will create a 'customerGrids' dictionary
        /// entry for each unique Controller/Action pair found in [CustomerGrids]
        /// Each entry will also have all of the columns associated with that grid.
        /// </summary>
        public  List<GridTemplateSet> GetGridTemplateSets(int customerId)
        {
            var list = new List<GridTemplateSet>();
            var templateGrids = new Dictionary<string, GridTemplateSet>();
            var customerGrids = new Dictionary<string, GridTemplate>();


            // Get the presently active grid data.  This will create a 'customerGrids' dictionary
            // entry for each unique Controller/Action pair found in [CustomerGrids]
            // Each entry will also have all of the columns associated with that grid.
            var activeGrids = from ag in RbacEntities.CustomerGrids
                              where ag.CustomerId == customerId
                              select new
                                  {
                                      GridId = 0,
                                      Controller = ag.Controller,
                                      Action = ag.Action,
                                      Title = ag.Title,
                                      Position = ag.Position,
                                      OriginalTitle = ag.OriginalTitle,
                                      OriginalPosition = ag.OriginalPosition,
                                      IsHidden = ag.IsHidden
                                  };
            foreach (var activeGrid in activeGrids)
            {
                string key = activeGrid.Controller + activeGrid.Action;
                if ( !customerGrids.ContainsKey( key ) )
                {
                    customerGrids.Add( key, new GridTemplate()
                        {
                            Controller = activeGrid.Controller,
                            Action = activeGrid.Action,
                            Selected = true
                        } );
                }
                customerGrids[key].Grid.Add(new GridData()
                    {
                        Title = activeGrid.Title,
                        Position = activeGrid.Position,
                        OriginalTitle = activeGrid.OriginalTitle,
                        OriginalPosition = activeGrid.OriginalPosition,
                        IsHidden = activeGrid.IsHidden
                    });
            }

            // Order the grid columns in each 'customerGrids' dictionary entry by Position
            foreach (var customerGrid in customerGrids)
            {
                customerGrid.Value.Grid.Sort();
            }

            //todo - GTC: Custom Grids
            // Get the available templates.
            //foreach (var grid in RbacEntities.CustomerGridTemplates)
            //{
            //    string customerGridsKey = grid.Controller + grid.Action;

            //    // Does this template set exist in dictionary?
            //    if ( !templateGrids.ContainsKey( customerGridsKey ) )
            //    {
            //        templateGrids.Add(customerGridsKey, new GridTemplateSet() { Controller = grid.Controller, Action = grid.Action, Version = grid.Version});    
            //        list.Add(templateGrids[customerGridsKey]);
            //    }

            //    var gi = new GridTemplate()
            //        {
            //            GridId = grid.CustomerGridTemplateId,
            //            Controller = grid.Controller,
            //            Action = grid.Action,
            //            Version = grid.Version,
            //            Selected = false
            //        };
            //    templateGrids[customerGridsKey].Add(gi);

            //    // Get the template columns
            //    var resFactory = new ResourceFactory();
            //    foreach (var col in grid.CustomerGridTemplateCols)
            //    {
            //        gi.Grid.Add(new GridData()
            //        {
            //            //do not localize the title, the admin is using this peice and doesnt care about what the customer has set their overriden value to.
            //            Title = col.Title,
            //            Position = col.Position,
            //            OriginalTitle = col.OriginalTitle,
            //            OriginalPosition = col.OriginalPosition,
            //            IsHidden =  col.IsHidden
            //        });
            //    }

            //    // Order the grid columns.
            //    gi.Grid.Sort();

            //    // Check if this grid set is selected.
            //    if ( customerGrids.ContainsKey( customerGridsKey ) )
            //    {
            //        // If the two grids match then this template is the selected one.
            //        if ( gi.Grid.Equals( customerGrids[customerGridsKey].Grid ) )
            //        {
            //            gi.Selected = true;
            //        }
            //    }
            //}

            return list;
        }

        /// <summary>
        /// Gets a grid template. used for custom grids
        /// </summary>
        public  GridTemplate GetGridTemplate(int templateId)
        {
            GridTemplate templateGrid = null;
            //todo - GTC: Custom Grids
            //var template = RbacEntities.CustomerGridTemplates.SingleOrDefault( m => m.CustomerGridTemplateId == templateId );

            //if ( template != null )
            //{

            //    templateGrid = new GridTemplate()
            //        {
            //            GridId = template.CustomerGridTemplateId,
            //            Controller = template.Controller,
            //            Action = template.Action,
            //            Version = template.Version,
            //            Selected = true
            //        };

            //    // Get the template columns
            //    foreach (var col in template.CustomerGridTemplateCols)
            //    {
            //        templateGrid.Grid.Add( new GridData()
            //            {
            //                //do not localize the title, the admin is using this peice and doesnt care about what the customer has set their overriden value to.
            //                Title = col.Title,
            //                Position = col.Position,
            //                OriginalTitle = col.OriginalTitle,
            //                OriginalPosition = col.OriginalPosition,
            //                IsHidden = col.IsHidden
            //            } );
            //    }
            //    // Order the grid columns
            //    templateGrid.Grid.Sort();

            //}

            return templateGrid;
        }

        /// <summary>
        /// Updates customer grid data with a localized version
        /// </summary>
        private  GridData UpdateColumnName(GridData gridData)
        {
            gridData.Title = (new ResourceFactory()).GetLocalizedTitle(ResourceTypes.GridColumn, gridData.Title);
            return gridData;
        }

        /// <summary>
        /// Gets localized titles for a griddata item for a specific customer, action, and controller. if nothing is found, returns the original title instead
        /// </summary>
        public  string GetLocalizedGridTitle(string controller, string action, int customerId, string originalTitle)
        {
            var item = RbacEntities.CustomerGrids.FirstOrDefault(x => x.CustomerId == customerId 
                                                                      && x.Controller == controller 
                                                                      && x.Action == action
                                                                      && x.OriginalTitle == originalTitle);
            return (new ResourceFactory()).GetLocalizedTitle(ResourceTypes.GridColumn, item != null ? item.Title : originalTitle);
        }

        /// <summary>
        /// Description: This Method will reorder postion , title and should be hidden or not
        /// ModifiedBy: Santhosh  (28/July/2014 - 04/Aug/2014) 
        /// </summary>
        /// <param name="GridRowdata"></param>
        /// <returns></returns>
        public int UpdateCustomGridDetails(GridController GridRowdata)
        {
            var CG = (from ag in RbacEntities.CustomerGrids
                      where ag.CustomerGridsId == GridRowdata.CustomerGridsId
                      select ag).FirstOrDefault();

            CG.Position = GridRowdata.Position;
            CG.Title = GridRowdata.Title;
            CG.IsHidden = GridRowdata.IsHidden;
            int num = RbacEntities.SaveChanges();
            return num;
        }

        /// <summary>
        /// Description: This Method will retrive controller list given to customer
        /// ModifiedBy: Santhosh  (28/July/2014 - 04/Aug/2014)  
        /// </summary>
        /// <param name="CustomerID"></param>
        /// <returns></returns>
        public IQueryable<GridController> GetController(int CustomerID)
        {
            var controller = (from ag in RbacEntities.CustomerGrids
                              where ag.CustomerId == CustomerID
                              select new GridController
                              {
                                  Controller = ag.Controller,
                              }).Distinct();
            return controller;
        }

        /// <summary>
        /// Description: This Method will retrive action list given to customer and Controllername
        /// ModifiedBy: Santhosh  (28/July/2014 - 04/Aug/2014) 
        /// </summary>
        /// <param name="CustomerID"></param>
        /// <param name="Controllername"></param>
        /// <returns></returns>
        public IQueryable<GridController> GetActionnames(int CustomerID, string Controllername)
        {
            var controlleraction = (from ag in RbacEntities.CustomerGrids
                                    where ag.CustomerId == CustomerID && ag.Controller == Controllername
                                    select new GridController
                                    {
                                        Action = ag.Action
                                    }).Distinct();
            return controlleraction;
        }

        /// <summary>
        /// Description: This Method will retrive grid postion, title, Ishidden
        /// ModifiedBy: Santhosh  (28/July/2014 - 04/Aug/2014) 
        /// </summary>
        /// <param name="CustomerID"></param>
        /// <param name="Controllername"></param>
        /// <param name="Actionname"></param>
        /// <returns></returns>
        public List<GridController> GetCustmerGrid(int CustomerID, string Controllername, string Actionname)
        {
            var ddlItems = new List<GridController>();
            List<GridController> controllergrid = new List<GridController>();

            var items = (from ag in RbacEntities.CustomerGrids
                         where ag.CustomerId == CustomerID && ag.Controller == Controllername && ag.Action == Actionname
                         select new GridController
                         {
                             CustomerGridsId = ag.CustomerGridsId,
                             Controller = ag.Controller,
                             Action = ag.Action,
                             Title = ag.Title,
                             Position = ag.Position,
                             OriginalTitle = ag.OriginalTitle,
                             OriginalPosition = ag.OriginalPosition,
                             IsHidden = ag.IsHidden

                         });
            controllergrid = items.ToList();

            if (controllergrid.Any())
                ddlItems = controllergrid;
            return ddlItems;
        }
      
    }
}
