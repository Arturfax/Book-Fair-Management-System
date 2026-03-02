using BookFair.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookFair.Core.DAO;

namespace BookFair.Core.Interfaces
{
    public interface IVisitorDAO
    {
            Visitor AddVisitor(Visitor visitor);
            Visitor? UpdateVisitor(Visitor visitor);
            Visitor? RemoveVisitor(int id);
            Visitor? GetVisitorById(int id);
            List<Visitor> GetAllVisitors(int page = 1, int pageSize = 50, string sortCriteria = "Id", SortDirection sortDirection = SortDirection.Ascending);
        
    }
}
