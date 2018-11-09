using iRentFeedData.Models.ModelView;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Security.Cryptography.Xml;
using System.Web;

namespace iRentFeedData.Models
{

    public class GetCoStarFeedData
    {
        public bool success { get; set; }

        // Connection to the DB
        public iRentEntities db = new iRentEntities();

        private const string URL = "http://costarxmlfeed.azurewebsites.net/api/coStarPost";
        //private const string URL = "http://localhost:52050/api/coStarPost";

        public GetCoStarFeedData(FeedDataModel data)
        {
            try
            {
                // Get Property
                var property = (from p in db.properties
                                where p.PropertyID == data.PropertyID
                                select p).FirstOrDefault();

                if (property != null)
                {
                    // Get Company
                    var company = (from c in db.companies
                                   where c.CompanyID == property.CompanyID
                                   select new { c.CompanyID, c.CompanyName }).FirstOrDefault();
                    ModelCompany comp = new ModelCompany();
                    comp.CompanyID = company.CompanyID;
                    comp.Name = company.CompanyName;

                    // Get Office Hours
                    List<ModelOfficeHour> offiHoursList = new List<ModelOfficeHour>();
                    var officeHours = (from oh in db.property_officehours
                                       where oh.PropertyID == data.PropertyID
                                       select oh).FirstOrDefault();
                    if (officeHours != null)
                    {
                        string SundayOpenTime = getOffiHourTime((int)officeHours.SundayOpenTime);
                        string SundayCloseTime = getOffiHourTime((int)officeHours.SundayCloseTime);
                        string SaturdayOpenTime = getOffiHourTime((int)officeHours.SaturdayOpenTime);
                        string SaturdayCloseTime = getOffiHourTime((int)officeHours.SaturdayCloseTime);
                        string MondayFridayOpenTime = getOffiHourTime((int)officeHours.MondayFridayOpenTime);
                        string MondayFridayCloseTime = getOffiHourTime((int)officeHours.MondayFridayCloseTime);
                        offiHoursList.Add(new ModelOfficeHour { Day = "Sunday", OpenTime = SundayOpenTime, CloseTime = SundayCloseTime });
                        offiHoursList.Add(new ModelOfficeHour { Day = "Monday", OpenTime = MondayFridayOpenTime, CloseTime = MondayFridayCloseTime });
                        offiHoursList.Add(new ModelOfficeHour { Day = "Tuesday", OpenTime = MondayFridayOpenTime, CloseTime = MondayFridayCloseTime });
                        offiHoursList.Add(new ModelOfficeHour { Day = "Wednesday", OpenTime = MondayFridayOpenTime, CloseTime = MondayFridayCloseTime });
                        offiHoursList.Add(new ModelOfficeHour { Day = "Thursday", OpenTime = MondayFridayOpenTime, CloseTime = MondayFridayCloseTime });
                        offiHoursList.Add(new ModelOfficeHour { Day = "Friday", OpenTime = MondayFridayOpenTime, CloseTime = MondayFridayCloseTime });
                        offiHoursList.Add(new ModelOfficeHour { Day = "Saturday", OpenTime = SaturdayOpenTime, CloseTime = SaturdayCloseTime });
                    }

                    // Get Parking
                    List<ModelParking> parkings = new List<ModelParking>();
                    var parkingList = (from p in db.unittypes
                                       where p.PropertyID == data.PropertyID
                                       select new { p.ParkingType }).Distinct().ToList();
                    foreach (var p in parkingList)
                    {
                        string parkingType = p.ParkingType;
                        bool reserved = false;
                        if (p.ParkingType == "Covered Parking - Reserved")
                        {
                            parkingType = "Covered Parking";
                            reserved = true;
                        }
                        if (p.ParkingType == "Covered Parking - Un-Reserved")
                        {
                            parkingType = "Covered Parking";
                            reserved = false;
                        }
                        if (p.ParkingType == "Uncovered Parking - Reserved")
                        {
                            parkingType = "Uncovered Parking";
                            reserved = true;
                        }
                        if (p.ParkingType == "Uncovered Parking - Un-Reserved")
                        {
                            parkingType = "Uncovered Parking";
                            reserved = false;
                        }
                        ModelParking mp = new ModelParking();
                        mp.ParkingType = parkingType;
                        mp.Assigned = reserved;
                        parkings.Add(mp);
                    }

                    // Get pets
                    // Check if is allowed
                    List<ModelPets> pets = new List<ModelPets>();
                    var petAllowed = (from p in db.unittypes
                                      where p.PropertyID == data.PropertyID && p.PetsAllowed != 3
                                      select new { p.UnitTypeID }).FirstOrDefault();
                    if (petAllowed != null)
                    {
                        ModelPets dog = new ModelPets();
                        dog.Allowed = true;
                        dog.PetType = "Dog";
                        dog.Count = 2;
                        dog.Restriction = "no";

                        ModelPets cat = new ModelPets();
                        cat.Allowed = true;
                        cat.PetType = "Cat";
                        cat.Count = 2;
                        cat.Restriction = "no";

                        pets.Add(dog);
                        pets.Add(cat);
                    }

                    // Get Floor Plans
                    List<ModelFloorPlan> floorPlans = new List<ModelFloorPlan>();
                    var floorPlanList = (from ut in db.unittypes
                                         where ut.PropertyID == data.PropertyID
                                         select ut).ToList();
                    foreach (var fp in floorPlanList)
                    {
                        // Get total Unit for this FloorPlan
                        var units = db.units.Where(u => u.UnitTypeID == fp.UnitTypeID);
                        int TotalUnits = units.Count();
                        int TotalUnitsAvailable = units.Count(u => u.Occupied != 1);

                        ModelFloorPlan modelFP = new ModelFloorPlan();
                        modelFP.FloorPlanID = fp.UnitTypeID;
                        modelFP.Name = fp.UnitTypeDescription;
                        modelFP.UnitCount = TotalUnits;
                        modelFP.UnitsAvailable = TotalUnitsAvailable;
                        modelFP.Bedrooms = Int32.Parse(fp.TotalBedrooms);
                        modelFP.Bathrooms = float.Parse(fp.TotalBathrooms);
                        modelFP.Sqft = Int32.Parse(fp.SquareFootage);
                        modelFP.Rent = fp.UnitCharge;
                        // Images
                        List<ModelFile> floorPlanImages = new List<ModelFile>();
                        // get List of images
                        var unitTypePhotos = db.unitphotos.Where(u => u.UnitTypeID == fp.UnitTypeID && u.MarketingFileTypeID == 1);
                        int rankFloorPlan = 0;
                        foreach (var photo in unitTypePhotos)
                        {
                            ModelFile floorPlanPhoto = new ModelFile();
                            floorPlanPhoto.Active = true;
                            floorPlanPhoto.FileID = photo.UnitPhotosID;
                            floorPlanPhoto.FileType = "FloorPlan";
                            floorPlanPhoto.Format = "Format2";
                            floorPlanPhoto.Rank = rankFloorPlan;
                            floorPlanPhoto.SRC = "http://www.myirent.com/rent/UnitPhotos/" + data.PropertyID.ToString() + "/" + photo.FileName;
                            rankFloorPlan++;
                            floorPlanImages.Add(floorPlanPhoto);
                        }
                        modelFP.Images = floorPlanImages;

                        floorPlans.Add(modelFP);
                    }

                    // Get Units
                    List<ModelUnit> unitListModel = new List<ModelUnit>();
                    // Check if get all or specific units
                    var unitList = new List<unit>();
                    if (data.Units == null)
                    {
                        unitList = (from u in db.units
                                        where u.PropertyID == data.PropertyID && u.Occupied == 0
                                        select u).ToList();
                    } else
                    {
                        unitList = db.units.Where(u => data.Units.Contains(u.UnitID)).ToList();
                    }
                    foreach (var unit in unitList)
                    {
                        ModelUnit newUnit = new ModelUnit();
                        newUnit.UnitID = unit.UnitID;
                        newUnit.MarketingName = unit.UnitName;
                        DateTime oDate = DateTime.Parse(unit.VacantDate.ToString());
                        newUnit.AvailabilityDay = oDate.Day.ToString();
                        newUnit.AvailabilityMonth = oDate.Month.ToString();
                        newUnit.AvailabilityYear = oDate.Year.ToString();
                        // Get Floor Plan for this unit
                        var getFloorPlan = (from fp in db.unittypes
                                            where fp.UnitTypeID == unit.UnitTypeID
                                            select fp).FirstOrDefault();
                        newUnit.FloorPlanID = getFloorPlan.UnitTypeID;
                        newUnit.UnitBedrooms = Int32.Parse(getFloorPlan.TotalBedrooms);
                        newUnit.UnitBathrooms = float.Parse(getFloorPlan.TotalBathrooms);
                        newUnit.MinSquareFeet = Int32.Parse(getFloorPlan.SquareFootage);
                        newUnit.MaxSquareFeet = Int32.Parse(getFloorPlan.SquareFootage);
                        newUnit.UnitRent = getFloorPlan.UnitCharge;
                        newUnit.FloorplanName = getFloorPlan.UnitTypeDescription;
                        newUnit.Deposit = property.NRSecurityDeposit + property.SecurityDeposit;
                        newUnit.EffectiveRent = getFloorPlan.UnitCharge;
                        unitListModel.Add(newUnit);
                    }

                    // Get Utilities
                    ModelUtilities utilities = new ModelUtilities();
                    utilities.Gas = property.Gas == 1 ? true : false;
                    utilities.Eletric = property.Electricity == 1 ? true : false;
                    utilities.Heat = false;
                    utilities.Sewer = false;
                    utilities.Trash = property.Trash == 1 ? true : false;
                    utilities.Water = property.Water == 1 ? true : false;

                    // Get Property Images
                    List<ModelFile> propertyImages = new List<ModelFile>();
                    var getPropertyPhotos = (from up in db.unitphotos
                                             where db.unittypes.Any(ut => ut.PropertyID == data.PropertyID && ut.UnitTypeID == up.UnitTypeID) && 
                                             up.MarketingFileTypeID == 2
                                             select up).ToList();
                    int rank = 0;
                    foreach (var propImage in getPropertyPhotos)
                    {
                        ModelFile propertyImage = new ModelFile();
                        propertyImage.FileID = propImage.UnitPhotosID;
                        propertyImage.Active = true;
                        propertyImage.FileType = "Photo";
                        propertyImage.Caption = null;
                        propertyImage.Format = null;
                        propertyImage.Rank = rank;
                        propertyImage.SRC = "http://www.myirent.com/rent/UnitPhotos/" + data.PropertyID.ToString() + "/" + propImage.FileName;
                        rank++;

                        propertyImages.Add(propertyImage);
                    }

                    // Set Property Model data
                    ModelProperty propertyData = new ModelProperty();
                    propertyData.Company = comp;
                    propertyData.PropertyID = data.PropertyID;
                    propertyData.Name = property.PropertyName;
                    propertyData.WebSite = property.PropertyWebsite;
                    propertyData.Address1 = property.PropertyAddress1;
                    propertyData.Address2 = property.PropertyAddress2;
                    propertyData.City = property.PropertyCity;
                    propertyData.State = property.PropertyState;
                    propertyData.PostalCode = property.PropertyZip;
                    propertyData.ILS_Identification = "Apartment"; // Look at this
                    var getTotalUnits = db.units.Where(u => u.PropertyID == property.PropertyID);
                    propertyData.TotalUnits = getTotalUnits.Count();
                    propertyData.OfficeHours = offiHoursList;
                    propertyData.LongDescription = property.PropertyLongDescription;
                    // Lease Duration
                    var leaseDuration = db.unittypes.Where(ut => ut.PropertyID == data.PropertyID).FirstOrDefault();
                    propertyData.LeaseLength = Int32.Parse(leaseDuration.LeaseDurationMonth);
                    propertyData.ParkingList = parkings;
                    if(property.AdminFee > 0)
                    {
                        propertyData.AdminFee = property.AdminFee;
                    }
                    propertyData.ApplicationFee = property.ApplicationFee;
                    propertyData.NonRefundableHoldFee = propertyData.NonRefundableHoldFee;
                    propertyData.Pets = pets;
                    propertyData.FloorPlans = floorPlans;
                    propertyData.Units = unitListModel;
                    propertyData.Utilities = utilities;
                    propertyData.PropertyImages = propertyImages;
                    propertyData.EmailSendXML = "gperazzo@myirent.com";

                    HttpClient client = new HttpClient();
                    client.BaseAddress = new Uri(URL);

                    var myContent = JsonConvert.SerializeObject(propertyData);
                    var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
                    var byteContent = new ByteArrayContent(buffer);
                    byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                    var result = client.PostAsync("", byteContent).Result;
                    var contents = result.Content.ReadAsStringAsync().Result; 

                    success = true;
                }

            }
            catch (Exception any)
            {
                Console.Write(any.ToString());

                MailMessage mailMessage = new MailMessage();
                mailMessage.To.Add("gperazzo@myirent.com");
                mailMessage.From = new MailAddress("support@myirent.com");
                mailMessage.Subject = "CoStar XML Feed - Error";
                mailMessage.Body = any.ToString();

                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.myirent.com"; //Or Your SMTP Server Address
                smtp.Port = 587;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new System.Net.NetworkCredential
                ("support@myirent.com", "iRent4Now!");
                smtp.Send(mailMessage);

                success = false;
            }

        }

        private string getOffiHourTime(int time)
        {
            if (time < 0)
            {
                return "Closed";
            }
            else if (time == 0)
            {
                return "12:00 AM";
            }
            else if(time == 12)
            {
                return "12:00 PM";
            }
            else if (time > 11)
            {
                int pmTime = time - 12;
                return (pmTime.ToString() + ":00 PM");
            }
            else
            {
                return (time.ToString() + ":00 AM");
            }
        }
    }
}