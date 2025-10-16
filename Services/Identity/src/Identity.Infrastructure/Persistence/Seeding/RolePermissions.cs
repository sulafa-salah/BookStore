using BookStore.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Infrastructure.Persistence.Seeding;


public static class RolePermissions
{
  
    public const string Admin = "Admin";
    public const string Manager = "Manager";          // broad ops manage
    public const string ContentManager = "ContentManager";   // catalog/content
    public const string OrderManager = "OrderManager";     // order ops
    public const string InventoryManager = "InventoryManager";
    public const string ShippingManager = "ShippingManager";
    public const string PaymentOps = "PaymentOps";
    public const string SupportAgent = "SupportAgent";
    public const string DashboardViewer = "DashboardViewer";  // read-only dashboard
    public const string DashboardAdmin = "DashboardAdmin";   // manage dashboard
    public const string Customer = "Customer";

 
    public const string CheckoutSvc = "checkout-svc";
    public const string CartSvc = "cart-svc";
    public const string OrdersSvc = "orders-svc";
    public const string PaymentSvc = "payment-svc";
    public const string InventorySvc = "inventory-svc";
    public const string ShippingSvc = "shipping-svc";
    public const string DashboardSvc = "dashboard-svc";

    public static IReadOnlyDictionary<string, IReadOnlyList<string>> Map
        => new Dictionary<string, IReadOnlyList<string>>
        {
          
            [Admin] = Permissions.GetAll(), // simple: admin gets everything explicitly

            [Manager] = new[]
            {
                // dashboard & analytics
                Permissions.DashboardRead, Permissions.DashboardAnalytics,

                // catalog
                Permissions.ProductsRead, Permissions.ProductsWrite, Permissions.ProductsManageCategories,

                // orders
                Permissions.OrdersRead, Permissions.OrdersUpdateStatus, Permissions.OrdersProcessRefunds, Permissions.OrdersViewAll,

                // inventory
                Permissions.InventoryRead, Permissions.InventoryWrite, Permissions.InventoryUpdateStock,

                // shipping
                Permissions.ShippingRead, Permissions.ShippingFulfill, Permissions.ShippingCreateLabel, Permissions.ShippingUpdate,

                // payments
                Permissions.PaymentsProcess, Permissions.PaymentsRefund, Permissions.PaymentsViewTransactions,

                // identity (limited)
                Permissions.UsersRead
            },

            [ContentManager] = new[]
            {
                Permissions.ProductsRead, Permissions.ProductsWrite,
                Permissions.ProductsManageCategories, Permissions.ProductsManageInventory,
                Permissions.DashboardRead
            },

            [OrderManager] = new[]
            {
                Permissions.OrdersRead, Permissions.OrdersUpdateStatus, Permissions.OrdersProcessRefunds,
                Permissions.OrdersViewAll,
                Permissions.PaymentsRefund,      // often needed to action refunds
                Permissions.DashboardRead
            },

            [InventoryManager] = new[]
            {
                Permissions.InventoryRead, Permissions.InventoryWrite, Permissions.InventoryUpdateStock,
                Permissions.ProductsManageInventory, // if inventory touches product stock
                Permissions.DashboardRead
            },

            [ShippingManager] = new[]
            {
                Permissions.ShippingRead, Permissions.ShippingCreateLabel, Permissions.ShippingFulfill,
                Permissions.ShippingTrack, Permissions.ShippingUpdate, Permissions.ShippingCarrierManage,
                Permissions.DashboardRead
            },

            [PaymentOps] = new[]
            {
                Permissions.PaymentsViewTransactions, Permissions.PaymentsProcess, Permissions.PaymentsRefund,
                Permissions.DashboardRead
            },

            [SupportAgent] = new[]
            {
                // help customers
                Permissions.CartAnyRead, Permissions.CartAnyWrite,
                Permissions.OrdersRead, Permissions.OrdersUpdateStatus, Permissions.OrdersViewAll,

                // look but don’t edit catalog or payments by default
                Permissions.ProductsRead,

                Permissions.DashboardRead
            },

            [DashboardViewer] = new[]
            {
                Permissions.DashboardRead, Permissions.DashboardAnalytics
            },

            [DashboardAdmin] = new[]
            {
                Permissions.DashboardRead, Permissions.DashboardAnalytics, Permissions.DashboardManage
            },

            [Customer] = new[]
            {
                // browse & cart
                Permissions.ProductsRead,
                Permissions.CartSelfRead, Permissions.CartSelfWrite,

                // orders
                Permissions.OrdersViewOwn, Permissions.OrdersWrite,

                // shipping (tracking)
                Permissions.ShippingTrack
            },

            // ===== Service Accounts (client credentials) =====
            [CheckoutSvc] = new[]
            {
                Permissions.CartAnyRead,
                Permissions.InventoryRead, Permissions.InventoryUpdateStock, 
                Permissions.OrdersWrite, Permissions.OrdersUpdateStatus,
                Permissions.PaymentsProcess,
                Permissions.ShippingFulfill, // if checkout triggers shipping creation
            },

            [CartSvc] = new[]
            {
                Permissions.ProductsRead // to validate items/price snapshots
            },

            [OrdersSvc] = new[]
            {
                Permissions.OrdersRead, Permissions.OrdersUpdateStatus,
                Permissions.PaymentsViewTransactions,
                Permissions.ShippingFulfill, Permissions.ShippingUpdate
            },

            [PaymentSvc] = new[]
            {
                Permissions.PaymentsProcess, Permissions.PaymentsRefund, Permissions.PaymentsViewTransactions,
                Permissions.OrdersUpdateStatus
            },

            [InventorySvc] = new[]
            {
                Permissions.InventoryRead, Permissions.InventoryWrite, Permissions.InventoryUpdateStock
            },

            [ShippingSvc] = new[]
            {
                Permissions.ShippingFulfill, Permissions.ShippingCreateLabel, Permissions.ShippingUpdate, Permissions.ShippingTrack
            },

            [DashboardSvc] = new[]
            {
                Permissions.DashboardRead, Permissions.DashboardAnalytics
            }
        };
}
   