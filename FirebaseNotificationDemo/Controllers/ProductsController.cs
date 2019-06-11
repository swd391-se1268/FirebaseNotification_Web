using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FirebaseNotificationDemo.Data;
using FirebaseNotificationDemo.Models;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;

namespace FirebaseNotificationDemo.Controllers
{
    public class ProductsController : Controller
    {
        private const string androidDeviceToken =
            "eP5VVuHrwR0:APA91bEUkeX6YXviSin0lXkvJd57whI7yz04UBjpmYwHPqTexkBhdT-FYuQFLBuRYlzM8QrP78LpJMgymIiv91Dpx0di24Dtfap175zggMBlW20i9297OOTUtaYm0oyp0e03bCHKjqpD";

        private const string webDeviceToken =
            "etPkPEfyiYY:APA91bGW-U_mCCz6dYdaDEU-F20XQxAOmY_djvqnMp0E2SR4jD8Rb35xRf3FfFRdptI9EvZJMgzL-UfewBxzrnBWwWfSbeQ91X62DZxaD5xmjcYxqC6Ngr4faAvDAWq5DOTlHLV7Q9Eq";

        private readonly ApplicationDbContext _context;
        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Product.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        [HttpGet]
        public async Task<string> Notify(int? id)
        {
            var product = await _context.Product
                .FirstOrDefaultAsync(m => m.Id == id);

            FirebaseNotification noti = new FirebaseNotification();
            DataMessage data = new DataMessage()
            {
                Data = product,
                RegistrationIds = new List<string>() { androidDeviceToken, webDeviceToken },
                notification = new Notification()
                {
                    title = product.Name,
                    text = "Name: " + product.Name + "\nPrice: " + product.Price + "\nStore: " + product.Store
                }
            };
            string result = await noti.SendMessage(data);

            return result;
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Price,CreateTime,Store")] Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();

                //notify
                FirebaseNotification noti = new FirebaseNotification();
                DataMessage data = new DataMessage()
                {
                    Data = product,
                    RegistrationIds = new List<string>() { androidDeviceToken, webDeviceToken },
                    notification = new Notification()
                    {
                        title = product.Name,
                        text = "Name: " + product.Name + "\nPrice: " + product.Price + "\nStore: " + product.Store
                    }
                };
                await noti.SendMessage(data);
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Price,CreateTime,Store")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Product.FindAsync(id);
            _context.Product.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Product.Any(e => e.Id == id);
        }

    }

    class FirebaseNotification
    {
        private const string serverKey =
            "AAAAEu-Zt6M:APA91bH-YxdkBL7O9LfDIzOMww_ksc-rgcnhA3m63sbUwt4YMpSW8QfTfhgqBlxSCjhc8fXEIx-_KhB9FsJoxAtJ6lrlxK6QHfhiVpMt-f0PzLsP3jnVbAFEu66woUv52YAYFOrpV4LJ";

        private const string url = "https://fcm.googleapis.com/fcm/send";

        public async Task<string> SendMessage(DataMessage dataMessage)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "key=" + serverKey);
                string body = JsonConvert.SerializeObject(dataMessage);
                HttpContent content = new StringContent(body, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    return result;
                }
            }
            return null;
        }
    }


}
