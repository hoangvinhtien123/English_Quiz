using English_Quiz.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace English_Quiz.Controllers
{
    public class PermissionController : BaseController
    {
        private English_QuizEntities _context = new English_QuizEntities();
        // GET: Permission
        
        public ActionResult LoadTable()
        {
            return View(_context.Permissions.ToList());
        }
        [CheckPermission(PermissionName = "QuanLyPhanQuyen", Action = ConstantCommon.Action.View)]
        public ActionResult Index(string ddlVaiTro)
        {
            //Load selected list vai trò
            LoadViewBag();
            //Kiểm tra vai trò
            if (string.IsNullOrEmpty(ddlVaiTro))
            {
                return View();
            }
            int vaitroId = -1;
            int.TryParse(ddlVaiTro, out vaitroId);
            List<PermissionViewModel> lst = new List<PermissionViewModel>();
            if (vaitroId > 0)
            {
                //Lấy tất cả chức năng
                var lstChucNang = _context.Functions.ToList().OrderBy(x=>x.OrderNumber);

                foreach (var item in lstChucNang)
                {
                    PermissionViewModel obj = new PermissionViewModel();
                    if (CheckQuyenDaCap(vaitroId, item.Id))  //chức năng đã đc cấp
                    {
                        obj.Id = item.Id;
                        obj.TenChucNang = item.Function_Name;
                        obj.CapChucNang = true;
                        lst.Add(obj);
                    }
                    else //chức năng chưa được cấp
                    {
                        obj.Id = item.Id;
                        obj.TenChucNang = item.Function_Name;
                        obj.CapChucNang = false;
                        lst.Add(obj);
                    }
                }
            }

            return View(lst);
        }
        public ActionResult GetObjToEdit(string id, string vaitroid)
        {
            try
            {
                if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(vaitroid))
                {
                    int idcn = -1; //id phân quyền
                    int idvaitro = -1;//id vai trò
                    int.TryParse(id, out idcn);
                    int.TryParse(vaitroid, out idvaitro);
                    if (idcn > 0 && idvaitro > 0)
                    {
                        Permission obj = _context.Permissions.Where(p => p.Role_Id == idvaitro && p.Function_Id == idcn).First();
                        //Tạo đối tượng trả về
                        Permission objReturn = new Permission();
                        string tenvaitro = obj.Role.ROLE_NAME;
                        return Json(new { tenvaitro = obj.Function.Function_Name, chucnangthem = obj.Is_Add, chucnangsua = obj.Is_Edit, chucnangxoa = obj.Is_Delete,  chucnangxem = obj.Is_View , motavaitro = obj.Function.Description }, JsonRequestBehavior.AllowGet);
                    }
                }
                return Json(new Role(), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new JsonPostBack("Có lỗi xảy ra", false), JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult getActionByVaiTro(string id)
        {
            int vaitroId = -1;
            int.TryParse(id, out vaitroId);
            if (vaitroId > 0)
            {
                //Lấy tất cả chức năng
                var obj = _context.Permissions.Where(x => x.Role_Id == vaitroId).First();
                return Json(new { error = false, xem = obj.Is_View, sua = obj.Is_Edit, xoa = obj.Is_Delete, them = obj.Is_Add
}, JsonRequestBehavior.AllowGet);
            }
            return Json(new { error = true }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult AddAll(string id)
        {
            int vaitroId = -1;
            bool xem = Convert.ToBoolean(Request["xem"]);
            bool xoa = Convert.ToBoolean(Request["xoa"]);
            bool sua = Convert.ToBoolean(Request["sua"]);
            bool them = Convert.ToBoolean(Request["them"]);
            
            try
            {
                int.TryParse(id, out vaitroId);
                if (vaitroId > 0)
                {
                    DeleteAllByVaiTroId(vaitroId);

                    var lstChucNang = _context.Functions.ToList();
                    foreach (var item in lstChucNang)
                    {
                        Permission obj = new Permission();
                        obj.Function_Id = item.Id;
                        obj.Role_Id = vaitroId;
                        obj.Is_View = xem;
                        obj.Is_Edit = sua;
                        obj.Is_Delete = xoa;
                        obj.Is_Add = them;
                        
                        _context.Permissions
.Add(obj);
                    }
                    _context.SaveChanges();
                    return Json(new JsonPostBack("Xóa thành công", true), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                return Json(new JsonPostBack("Xóa thành công", false), JsonRequestBehavior.AllowGet);
            }
            return Json(new JsonPostBack("Xóa thành công", false), JsonRequestBehavior.AllowGet);
        }
        public JsonResult Add(string id, string itemId)
        {
            int vaitroId = -1;
            bool xem = Convert.ToBoolean(Request["xem"]);
            bool xoa = Convert.ToBoolean(Request["xoa"]);
            bool sua = Convert.ToBoolean(Request["sua"]);
            bool them = Convert.ToBoolean(Request["them"]);
            string[] lstIdChucNang;
            if (!string.IsNullOrEmpty(itemId))
            {
                lstIdChucNang = itemId.Split('-');
            }
            else
            {
                return Json(new JsonPostBack("Xóa thành công", false), JsonRequestBehavior.AllowGet);
            }
            try
            {
                int.TryParse(id, out vaitroId);
                if (vaitroId > 0)
                {
                    //Thêm mới quyền
                    foreach (var item in lstIdChucNang)
                    {
                        Permission obj = new Permission();
                        obj.Function_Id = int.Parse(item);
                        obj.Role_Id = vaitroId;
                        obj.Is_View = xem;
                        obj.Is_Edit = sua;
                        obj.Is_Delete = xoa;
                        obj.Is_Add = them;
                        
                        _context.Permissions.Add(obj);
                    }
                    _context.SaveChanges();
                    return Json(new JsonPostBack("Xóa thành công", true), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                return Json(new JsonPostBack("Xóa thành công", false), JsonRequestBehavior.AllowGet);
            }
            return Json(new JsonPostBack("Xóa thành công", false), JsonRequestBehavior.AllowGet);
        }
        public JsonResult Delete(string id, string itemId)
        {
            int vaitroId = -1;

            string[] lstIdChucNang;
            if (!string.IsNullOrEmpty(itemId))
            {
                lstIdChucNang = itemId.Split('-');
            }
            else
            {
                return Json(new JsonPostBack("Xóa thành công", false), JsonRequestBehavior.AllowGet);
            }
            try
            {
                int.TryParse(id, out vaitroId);
                if (vaitroId > 0)
                {
                    foreach (var item in lstIdChucNang)
                    {
                        int Idchucnang = int.Parse(item);
                        Permission obj = _context.Permissions.Where(x => x.Function_Id == Idchucnang && x.Role_Id == vaitroId).First();
                        _context.Permissions.Remove(obj);
                    }
                    _context.SaveChanges();
                    return Json(new JsonPostBack("Xóa thành công", true), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new JsonPostBack("Xóa thất bại", false), JsonRequestBehavior.AllowGet);
            }
            return Json(new JsonPostBack("Xóa thất bại", false), JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteAll(string id)
        {
            int itemId = -1;
            try
            {
                int.TryParse(id, out itemId);
                if (itemId > 0)
                {
                    DeleteAllByVaiTroId(itemId);
                    return Json(new JsonPostBack("Xóa thành công", true), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                return Json(new JsonPostBack("Xóa thành công", false), JsonRequestBehavior.AllowGet);
            }
            return Json(new JsonPostBack("Xóa thành công", false), JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveQuyen()
        {
            try
            {
                int vaitroid = -1;
                int chucnangid = -1;
                if (!string.IsNullOrEmpty(Request["idvt"]))
                {
                    int.TryParse(Request["idvt"], out vaitroid);
                }
                if (!string.IsNullOrEmpty(Request["idcn"]))
                {
                    int.TryParse(Request["idcn"], out chucnangid);
                }
                bool xem = Convert.ToBoolean(Request["xem"]);
                bool xoa = Convert.ToBoolean(Request["xoa"]);
                bool sua = Convert.ToBoolean(Request["sua"]);
                bool them = Convert.ToBoolean(Request["them"]);
                bool baocao = Convert.ToBoolean(Request["baocao"]);
                if (vaitroid >= 0 && chucnangid > 0)
                {
                    var obj = _context.Permissions.Where(p => p.Role_Id == vaitroid && p.Function_Id == chucnangid).First();
                    obj.Is_View = xem;
                    obj.Is_Edit = sua;
                    obj.Is_Delete = xoa;
                    obj.Is_Add = them;
                    _context.SaveChanges();
                    return Json(new JsonPostBack("Cập nhật thành công", true), JsonRequestBehavior.AllowGet);
                }
                return Json(new JsonPostBack("Xảy ra lỗi trong quá trình xử lý", false), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new JsonPostBack("Xảy ra lỗi trong quá trình xử lý", false), JsonRequestBehavior.AllowGet);
            }
        }

        private bool CheckQuyenDaCap(int vaitroId, int idchacnang)
        {
            var lstDaCapQuyen = _context.Permissions.Where(x => x.Role_Id == vaitroId).ToList();
            int dem = lstDaCapQuyen.Count;
            for (int i = 0; i < dem; i++)
            {
                if (idchacnang == lstDaCapQuyen[i].Function_Id)
                {
                    return true;
                }
            }
            return false;
        }
        private bool DeleteAllByVaiTroId(int vaitroid)
        {
            try
            {
                var lstObj = _context.Permissions.Where(x => x.Role_Id == vaitroid);
                _context.Permissions.RemoveRange(lstObj);
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public List<Permission> getObjByChucNang(string cn)
        {
            try
            {
                int id = -1;
                int.TryParse(cn, out id);
                if (id > 0)
                {
                    return _context.Permissions.Where(x => x.Function_Id == id).ToList();
                }
                return new List<Permission>();
            }
            catch (Exception)
            {
                return new List<Permission>();
            }
        }
        public void LoadViewBag()
        {
            var lstVaiTro = _context.Roles.ToList();
            ViewBag.ddlVaiTro = new SelectList(lstVaiTro, "Role_Id", "Role_Name");

            List<Function> lstchucnang = _context.Functions.ToList();
            //Láy chức năng
            StringBuilder sbd = new StringBuilder();
            for (int i = 0; i < lstchucnang.Count; i++)
            {
                sbd.Append("<option value=" + lstchucnang[i].Id + ">" + lstchucnang[i].Function_Name + "</option>");
            }

            ViewBag.ddlchucnang = sbd.ToString();
        }
        public JsonResult GetPermission()
        {
            Function function = _context.Functions.FirstOrDefault(x => string.Compare(x.Form_Name, "QuanLyCauHoi", true) == 0);
            int role = int.Parse(Session["Role"].ToString());
            Permission permission = _context.Permissions.FirstOrDefault(x => x.Role_Id == role && x.Function_Id == function.Id);
            return Json(new
            {
                Is_Add = permission.Is_Add,
                Is_Edit = permission.Is_Edit,
                Is_Delete = permission.Is_Delete
            }, JsonRequestBehavior.AllowGet);
        }
    }
}