using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WarehouseFileArchiverAPI.Interfaces;
using WarehouseFileArchiverAPI.Models;
using WarehouseFileArchiverAPI.Models.DTOs;

namespace WarehouseFileArchiverAPI.Mappers
{
    public class CategoryMapper 
    {
        public Category MapCategoryAddDtoToCategory(CategoryAddRequestDto category)
        {
            Category newCategory = new()
            {
                CategoryName = category.CategoryName
            };
            return newCategory;
        }
    }
}