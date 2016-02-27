using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckBook.DataAccess.Data
{
    public interface IAvatarData
    {

        int UserId { get; set; }

        string Name { get; set; }

        string ImageUrl { get; set; }

    }
}
