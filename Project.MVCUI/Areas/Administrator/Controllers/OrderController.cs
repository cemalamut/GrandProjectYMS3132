﻿using Project.BLL.DesignPatterns.RepositoryPattern.ConcRep;
using Project.MODEL.Entities;
using Project.MVCUI.AuthenticationClasses;
using Project.MVCUI.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project.MVCUI.Areas.Administrator.Controllers
{
    [ActFilter, ResFilter, AdminAuthentication]
    public class OrderController : Controller
    {
        #region Açıklama
        /*
         * Burası geliştirilecek, henüz karar verilmedi nasıl bir konsept olacağına. Admin sipariş sayfasında hangi verileri görecek bir araştır.
         * Kullanıcı siparişi verdikten sonra tüm siparişler admin paneline düşer ve adminin onayladığı siparişler hazırlanır ve gönderilir. Yani kullanıcının her sipariş vermesi ile olay bitmiyor. Dolayısıyla sipariş tablosunda onay property'sinin de açılması gerekir. Admin onay vermesi halinde kargonun gönderilmesi aksi halde kullanıcıya bilgi verilmesi gerekir. Buranın detaylarına gireceğiz bu nedenle sonraya bırakıldı.
         * 
         * 
         * 
         */
        #endregion

        OrderRepository orep;

        OrderDetailRepository odrep;

        public OrderController()
        {
            orep = new OrderRepository();

            odrep = new OrderDetailRepository();
        }

        public ActionResult List()
        {
            return View(Tuple.Create(orep.GetActives(), odrep.GetActives()));
        }

        public ActionResult Update(int id)
        {
            return View(Tuple.Create(orep.GetByID(id), odrep.FirstOrDefault(x => x.OrderID == id)));
        }

        public ActionResult Update([Bind(Prefix = "item1")]Order order, [Bind(Prefix = "item2")]OrderDetail od)
        {
            orep.Update(order);
            odrep.Update(od);
            return RedirectToAction("List");
        }

        public ActionResult Delete(int id)
        {
            Order order = orep.GetByID(id);
            orep.Delete(order);
            return RedirectToAction("List");
        }
    }
}
