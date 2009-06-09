using System;
using System.Web;
using DV_Enterprises.Web.Service.Interface;
using StructureMap;

namespace DV_Enterprises.Web.Service
{
    [Pluggable("Default")]
    public class Redirector : IRedirector
    {
        /// <summary>
        /// Go to default products page. This the product list page.
        /// </summary>
        public void GoToProducts()
        {
            Redirect("~/Products/Default.aspx");
        }

        /// <summary>
        /// Go to the product page
        /// </summary>
        /// <param name="productId"></param>
        public void GoToViewProduct(int productId)
        {
            Redirect("~/Products/Default.aspx");
        }

        /// <summary>
        /// Go to the manage product page. This is an edit form.
        /// </summary>
        /// <param name="productId">Product Id</param>
        public void GoToManageProduct(int productId)
        {
            Redirect(String.Format("~/Products/ManageProduct.aspx?ProductID={0}", productId));
        }

        /// <summary>
        /// Go to manage product page. This is for new products
        /// </summary>
        public void GoToManageProduct()
        {
            Redirect("~/Products/ManageProduct.aspx");
        }

        public void GoToGreenhouses()
        {
            Redirect("~/Greenhouses/Default.aspx");
        }

        public void GoToViewGreenhouse(int greenhouseId)
        {
            Redirect(String.Format("~/Greenhouses/ViewGreenhouse.aspx?GreenhouseID={0}", greenhouseId));
        }

        public void GoToHomePage()
        {
            Redirect("~/Default.aspx");
        }

        public void GoToErrorPage()
        {
            Redirect("~/Error.aspx");
        }

        public void GoToSignupPage()
        {
            Redirect("~/Accounts/Signup.aspx");
        }

        public void GoToAdminPage()
        {
            Redirect("~/Admin/Default.aspx");
        }

        public void GoToAdminUserPage(string username)
        {
            Redirect(String.Format("~/Admin/User.aspx?Username={0}", username));
        }

        private static void Redirect(string path)
        {
            HttpContext.Current.Response.Redirect(path);
        }
    }
}