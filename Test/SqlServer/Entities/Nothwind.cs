// *********************************************
// Code Generated with Apps72.Dev.Data.Generator
// *********************************************
using System;

namespace Data.Tests.Entities.Nothwind
{
    /// <summary />
    public partial class Categories
    {
        /// <summary />
        public virtual Int32 CategoryID { get; set; }
        /// <summary />
        public virtual String CategoryName { get; set; }
        /// <summary />
        public virtual String Description { get; set; }
        /// <summary />
        public virtual Byte[] Picture { get; set; }
    }
    /// <summary />
    public partial class CustomerCustomerDemo
    {
        /// <summary />
        public virtual String CustomerID { get; set; }
        /// <summary />
        public virtual String CustomerTypeID { get; set; }
    }
    /// <summary />
    public partial class CustomerDemographics
    {
        /// <summary />
        public virtual String CustomerTypeID { get; set; }
        /// <summary />
        public virtual String CustomerDesc { get; set; }
    }
    /// <summary />
    public partial class Customers
    {
        /// <summary />
        public virtual String CustomerID { get; set; }
        /// <summary />
        public virtual String CompanyName { get; set; }
        /// <summary />
        public virtual String ContactName { get; set; }
        /// <summary />
        public virtual String ContactTitle { get; set; }
        /// <summary />
        public virtual String Address { get; set; }
        /// <summary />
        public virtual String City { get; set; }
        /// <summary />
        public virtual String Region { get; set; }
        /// <summary />
        public virtual String PostalCode { get; set; }
        /// <summary />
        public virtual String Country { get; set; }
        /// <summary />
        public virtual String Phone { get; set; }
        /// <summary />
        public virtual String Fax { get; set; }
    }
    /// <summary />
    public partial class Employees
    {
        /// <summary />
        public virtual Int32 EmployeeID { get; set; }
        /// <summary />
        public virtual String LastName { get; set; }
        /// <summary />
        public virtual String FirstName { get; set; }
        /// <summary />
        public virtual String Title { get; set; }
        /// <summary />
        public virtual String TitleOfCourtesy { get; set; }
        /// <summary />
        public virtual DateTime BirthDate { get; set; }
        /// <summary />
        public virtual DateTime HireDate { get; set; }
        /// <summary />
        public virtual String Address { get; set; }
        /// <summary />
        public virtual String City { get; set; }
        /// <summary />
        public virtual String Region { get; set; }
        /// <summary />
        public virtual String PostalCode { get; set; }
        /// <summary />
        public virtual String Country { get; set; }
        /// <summary />
        public virtual String HomePhone { get; set; }
        /// <summary />
        public virtual String Extension { get; set; }
        /// <summary />
        public virtual Byte[] Photo { get; set; }
        /// <summary />
        public virtual String Notes { get; set; }
        /// <summary />
        public virtual Int32 ReportsTo { get; set; }
        /// <summary />
        public virtual String PhotoPath { get; set; }
    }
    /// <summary />
    public partial class EmployeeTerritories
    {
        /// <summary />
        public virtual Int32 EmployeeID { get; set; }
        /// <summary />
        public virtual String TerritoryID { get; set; }
    }
    /// <summary />
    public partial class OrderDetails
    {
        /// <summary />
        public virtual Int32 OrderID { get; set; }
        /// <summary />
        public virtual Int32 ProductID { get; set; }
        /// <summary />
        public virtual Decimal UnitPrice { get; set; }
        /// <summary />
        public virtual Int16 Quantity { get; set; }
        /// <summary />
        public virtual Double Discount { get; set; }
    }
    /// <summary />
    public partial class Orders
    {
        /// <summary />
        public virtual Int32 OrderID { get; set; }
        /// <summary />
        public virtual String CustomerID { get; set; }
        /// <summary />
        public virtual Int32 EmployeeID { get; set; }
        /// <summary />
        public virtual DateTime OrderDate { get; set; }
        /// <summary />
        public virtual DateTime RequiredDate { get; set; }
        /// <summary />
        public virtual DateTime ShippedDate { get; set; }
        /// <summary />
        public virtual Int32 ShipVia { get; set; }
        /// <summary />
        public virtual Decimal Freight { get; set; }
        /// <summary />
        public virtual String ShipName { get; set; }
        /// <summary />
        public virtual String ShipAddress { get; set; }
        /// <summary />
        public virtual String ShipCity { get; set; }
        /// <summary />
        public virtual String ShipRegion { get; set; }
        /// <summary />
        public virtual String ShipPostalCode { get; set; }
        /// <summary />
        public virtual String ShipCountry { get; set; }
    }
    /// <summary />
    public partial class Products
    {
        /// <summary />
        public virtual Int32 ProductID { get; set; }
        /// <summary />
        public virtual String ProductName { get; set; }
        /// <summary />
        public virtual Int32 SupplierID { get; set; }
        /// <summary />
        public virtual Int32 CategoryID { get; set; }
        /// <summary />
        public virtual String QuantityPerUnit { get; set; }
        /// <summary />
        public virtual Decimal UnitPrice { get; set; }
        /// <summary />
        public virtual Int16 UnitsInStock { get; set; }
        /// <summary />
        public virtual Int16 UnitsOnOrder { get; set; }
        /// <summary />
        public virtual Int16 ReorderLevel { get; set; }
        /// <summary />
        public virtual Boolean Discontinued { get; set; }
    }
    /// <summary />
    public partial class Region
    {
        /// <summary />
        public virtual Int32 RegionID { get; set; }
        /// <summary />
        public virtual String RegionDescription { get; set; }
    }
    /// <summary />
    public partial class Shippers
    {
        /// <summary />
        public virtual Int32 ShipperID { get; set; }
        /// <summary />
        public virtual String CompanyName { get; set; }
        /// <summary />
        public virtual String Phone { get; set; }
    }
    /// <summary />
    public partial class Suppliers
    {
        /// <summary />
        public virtual Int32 SupplierID { get; set; }
        /// <summary />
        public virtual String CompanyName { get; set; }
        /// <summary />
        public virtual String ContactName { get; set; }
        /// <summary />
        public virtual String ContactTitle { get; set; }
        /// <summary />
        public virtual String Address { get; set; }
        /// <summary />
        public virtual String City { get; set; }
        /// <summary />
        public virtual String Region { get; set; }
        /// <summary />
        public virtual String PostalCode { get; set; }
        /// <summary />
        public virtual String Country { get; set; }
        /// <summary />
        public virtual String Phone { get; set; }
        /// <summary />
        public virtual String Fax { get; set; }
        /// <summary />
        public virtual String HomePage { get; set; }
    }
    /// <summary />
    public partial class Territories
    {
        /// <summary />
        public virtual String TerritoryID { get; set; }
        /// <summary />
        public virtual String TerritoryDescription { get; set; }
        /// <summary />
        public virtual Int32 RegionID { get; set; }
    }
}
