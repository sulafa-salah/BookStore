using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Permissions;
    public static class Permissions
    {
        // Claim type used everywhere
        public const string ClaimType = "Permission";

        // ===== User / Identity (admin console) =====
        public const string UsersRead = "users:read";
        public const string UsersWrite = "users:write";
        public const string UsersDelete = "users:delete";
        public const string UsersManageRoles = "users:manage_roles";

        // ===== Catalog / Products =====
        public const string ProductsRead = "products:read";
        public const string ProductsWrite = "products:write";
        public const string ProductsDelete = "products:delete";
        public const string ProductsManageCategories = "products:manage_categories";
        public const string ProductsManageInventory = "products:manage_inventory"; 

        // ===== Cart (Shopping Basket) =====
        public const string CartSelfRead = "cart:self.read";
        public const string CartSelfWrite = "cart:self.write";
        public const string CartAnyRead = "cart:any.read";
        public const string CartAnyWrite = "cart:any.write";

        // ===== Orders =====
        public const string OrdersRead = "orders:read";           
        public const string OrdersWrite = "orders:write";          
        public const string OrdersDelete = "orders:delete";
        public const string OrdersUpdateStatus = "orders:update_status";
        public const string OrdersProcessRefunds = "orders:process_refunds";
        public const string OrdersViewAll = "orders:view_all";
        public const string OrdersViewOwn = "orders:view_own";

        // ===== Payments =====
        public const string PaymentsProcess = "payments:process";
        public const string PaymentsRefund = "payments:refund";
        public const string PaymentsViewTransactions = "payments:view_transactions";

        // ===== Inventory =====
        public const string InventoryRead = "inventory:read";
        public const string InventoryWrite = "inventory:write";
        public const string InventoryUpdateStock = "inventory:update_stock";

        // ===== Shipping  =====
        public const string ShippingRead = "shipping:read";          // shipments, carriers, methods
        public const string ShippingCreateLabel = "shipping:label.create";  // buy/print label
        public const string ShippingFulfill = "shipping:fulfill";       // mark as shipped / create shipment
        public const string ShippingTrack = "shipping:track";         // track parcels
        public const string ShippingUpdate = "shipping:update";        // edit address/shipments
        public const string ShippingCarrierManage = "shipping:carrier.manage";// add/edit carriers & rates

        // ===== Dashboard (admin web app) =====
        public const string DashboardRead = "dashboard:read";          // access dashboard UI
        public const string DashboardManage = "dashboard:manage";        // manage widgets, sections, access
        public const string DashboardAnalytics = "dashboard:analytics.read";// analytics/BI widgets

        // ===== Admin (superuser) =====
        public const string AdminFullAccess = "admin:full_access";     
                                                                       

        public static IReadOnlyList<string> GetAll() => new[]
        {
        // users
        UsersRead, UsersWrite, UsersDelete, UsersManageRoles,

        // catalog
        ProductsRead, ProductsWrite, ProductsDelete, ProductsManageCategories, ProductsManageInventory,

        // cart
        CartSelfRead, CartSelfWrite, CartAnyRead, CartAnyWrite,

        // orders
        OrdersRead, OrdersWrite, OrdersDelete, OrdersUpdateStatus, OrdersProcessRefunds, OrdersViewAll, OrdersViewOwn,

        // payments
        PaymentsProcess, PaymentsRefund, PaymentsViewTransactions,

        // inventory
        InventoryRead, InventoryWrite, InventoryUpdateStock,

        // shipping
        ShippingRead, ShippingCreateLabel, ShippingFulfill, ShippingTrack, ShippingUpdate, ShippingCarrierManage,

        // dashboard
        DashboardRead, DashboardManage, DashboardAnalytics,

        // admin
        AdminFullAccess
    };
    }