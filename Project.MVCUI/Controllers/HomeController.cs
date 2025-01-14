﻿using Project.BLL.DesignPatterns.RepositoryPattern.ConcRep;
using Project.COMMON.CommonTools;
using Project.MODEL.Entities;
using Project.MODEL.Enums;
using Project.MVCUI.AuthenticationClasses;
using Project.MVCUI.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project.MVCUI.Controllers
{
    //[ActFilter, ResFilter,MemberAuthentication]
    public class HomeController : Controller
    {
        #region Açıklama
        /*
         * Burada üyelik için gerekli işlemler tanımlandı. Buraya sitenin ön tarafından kayıt olmak isteyen veya giriş yapmak isteyen kullanıcılar erişecek. Bu nedenle kullanıcının sadece member olması durumunda giriş yapılmasına iziin verildi. Admin burardan giriş yapmamalı, yapamamalıdır.
         * 
         */
        #endregion

        AppUserRepository arep;

        AppUserDetailRepository adrep;

        public HomeController()
        {
            arep = new AppUserRepository();

            adrep = new AppUserDetailRepository();
        }

        public ActionResult Register()
        {
            return View();          //Kullanıcının üyelik işlemleri için açılacak sayfa
        }   //Üye kayıt işlemleri

        [HttpPost]
        public ActionResult Register([Bind(Prefix = "item1")]AppUser item, [Bind(Prefix = "item2")]AppUserDetail item2)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            string mesaj = arep.CheckCredentials(item.UserName, item.Email, out bool varmi);

            if (varmi == false)
            {
                item.Role = UserRole.Member;
                arep.Add(item);
                item2.AppUser = item;
                adrep.Add(item2);

                MailSender.Send(item.Email, body: $"{"https://localhost:44377/Home/RegisterOnay/"}{item.ActivationCode}", subject: "Doğrulama Kodu");

                ViewBag.mailOnay = "Mail adresinize gelen aktivasyon linkini tıklayın.";

                return View();
            }

            ViewBag.ZatenVar = mesaj;

            return View();
        }

        public ActionResult RegisterOnay(Guid id)
        {
            AppUser kullanici = arep.FirstOrDefault(x => x.ActivationCode == id);

            if (kullanici != null)
            {
                kullanici.IsActive = true;
                arep.Update(kullanici);
                TempData.Add("HesapAktif", $"{kullanici.UserName} Hesabınız Zaten Aktif");
                return RedirectToAction("Login"); // todo: sonradan eklendi


            }
            ViewBag.mailonay = "Doğrulama Yapılamadı";
            return RedirectToAction("Register");
        }   //Üye onay işlemleri

        public ActionResult Login()
        {
            return View();
        }   //Üye giriş işlemleri

        [HttpPost]
        public ActionResult Login(AppUser item)
        {
            if (arep.KontrolEt(item.UserName,item.Password) != null )
            {
                Session.Add("member",arep.KontrolEt(item.UserName,item.Password));
                return RedirectToAction("ProductList", "Member");  // todo: sonradan eklendi
            }
            else
            {
                ViewBag.Hatali = "Kullanıcı Bilgileri Hatalı. Kayıtlı Değilseniz: ";
                return View();
            }
        }
    }
}

