﻿using HyperStoreService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using HyperStoreServiceAPP.DTO.CartManagementDTO;
using System.Data.Entity;

namespace HyperStoreServiceAPP.Controllers.CartManagementAPI
{
    public class CartsController : ApiController, ICart
    {
        private HyperStoreServiceContext db;

        public IHttpActionResult CompleteTheOrderInLiveCart(Guid userId)
        {
            throw new NotImplementedException();
        }

        [HttpPut]
        public IHttpActionResult UpdateLiveCartMetadata(Guid userId, UpdateLiveCartDTO updateLiveCartDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (updateLiveCartDTO == null)
                throw new Exception("updateLiveCartDTO should not have been null.");

            db = UtilityAPI.RetrieveDBContext(userId);

            var cart = _RetrievesLiveCart((Guid)updateLiveCartDTO.PersonId);

            if (cart == null)
                throw new Exception("There is no live cart while updating the cart metadata.");

            if (updateLiveCartDTO.IsBookMark != null)
                cart.IsBookmarked = updateLiveCartDTO.IsBookMark;

            if (updateLiveCartDTO.PreferedDeliveryTime != null)
                cart.PreferedDeliveryTime = updateLiveCartDTO.PreferedDeliveryTime;

            db.Entry(cart).State = EntityState.Modified;

            db.SaveChangesAsync();

            return Ok(cart);
        }

        /// <summary>
        /// Retrieves the live cart, if there is no live cart, it will create new live cart.
        /// </summary>
        /// <param name="personId"></param>
        /// <returns></returns>
        private Cart _RetrievesLiveCart(Guid personId)
        {
            var cart = db.Carts.Where(c => c.CartStatus != CartStatus.OrderCompleted &&
                                            c.PersonId == personId)
                                .FirstOrDefault();
            return cart;
        }
    }
}
