using Microsoft.AspNetCore.Mvc;
using SQLServerDeneme.DataAccessLayer;
using SQLServerDeneme.Models;

namespace SQLServerDeneme.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly Employee_DAL _dal;

        public EmployeeController(Employee_DAL dal)
        {
            _dal = dal;
        }
        [HttpGet]
        public IActionResult Index()
        {
            List<Employee> employees = new List<Employee>();
            try
            {
                employees = _dal.GetAll();
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message ;
            }
            return View(employees);

        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Employee model)
        {
            if (!ModelState.IsValid)
            {
                TempData["errorMessage"] = "Model data is invalid";
                return View();
            }

            try
            {
                bool result = _dal.CreateEmployee(model);

                if (!result)
                {
                    TempData["errorMessage"] = "Unable to save the data";
                    return View();
                }

                TempData["successMessage"] = "Employee details saved";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = "An error occurred while saving the data: " + ex.Message;
                return View();
            }
        }
        public IActionResult Edit(int id)
        {
            var employee = _dal.GetEmployeeById(id);
            if (employee == null)
            {
                return NotFound();
            }
            return View(employee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Employee employee)
        {
            if (id != employee.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                bool result = _dal.UpdateEmployee(employee);
                if (result)
                {
                    TempData["successMessage"] = "Employee details updated successfully";
                    return RedirectToAction("Index");
                }
                TempData["errorMessage"] = "Unable to update the employee details";
            }

            return View(employee);
        }
        public IActionResult Delete(int id)
        {
            var employee = _dal.GetEmployeeById(id);
            if (employee == null)
            {
                return NotFound();
            }
            return View(employee);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id, Employee employee)
        {
            if (id != employee.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                bool result = _dal.DeleteEmployee(id);
                if (result)
                {
                    TempData["successMessage"] = "Employee details updated successfully";
                    return RedirectToAction("Index");
                }
                TempData["errorMessage"] = "Unable to update the employee details";
            }

            return View(employee);
        }

    }
}

