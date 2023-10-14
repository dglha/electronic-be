﻿using Electronic.Domain.Common;
using Electronic.Domain.Model.Catalog;

namespace Electronic.Domain.Models.Catalog;

public class ProductOptionGroup : BaseEntity
{
    public int ProductOptionGroupId { get; set; }
    public long ProductId { get; set; }
    public Product Product { get; set; }
    public int ProductOptionId { get; set; }
    public ProductOption ProductOption { get; set; }
    public string Value { get; set; }
}