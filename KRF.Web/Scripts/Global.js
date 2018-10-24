var data;
debugger;
var jQuery_11 = jQuery.noConflict(true)
jQuery_11(document).ready(function () {
    alert('Enter');
            jQuery_11('a.check').click(function () {
                jQuery_11(this).toggleClass("down");
            });
            
            //--------------------------global
            jQuery_11("#searchPopup").on("click", function (e) {
                e.stopPropagation();
            });
            jQuery_11("#glbsearch").on("click", function (e) {
                e.stopPropagation();
                jQuery_11("#searchPopup").show("fast");
                var ele = document.getElementById("glbsearch");
                var top = 0;
                var left = 0;
                var ele = document.getElementById("glbsearch");

                while (ele.tagName != "BODY") {
                    top += ele.offsetTop;
                    left += ele.offsetLeft;
                    ele = ele.offsetParent;
                }
                document.getElementById("searchPopup").style.top = (top + 10) + "px";
                document.getElementById("searchPopup").style.left = (left - 310) + "px";
            });
            jQuery_11("#accountPopup").on("click", function (e) {
                e.stopPropagation();
            });
            jQuery_11("#accountDD").on("click", function (e) {
                e.stopPropagation();
                jQuery_11("#accountPopup").show("fast");
                var ele = document.getElementById("accountDD");
                var top = 0;
                var left = 0;
                var ele = document.getElementById("accountDD");
                while (ele.tagName != "BODY") {
                    top += ele.offsetTop;
                    left += ele.offsetLeft;
                    ele = ele.offsetParent;
                }
                document.getElementById("accountPopup").style.top = (top + 10) + "px";
                document.getElementById("accountPopup").style.left = (left - 106) + "px";
            });
            jQuery_11(document).on("click", function () {
                jQuery_11("#searchPopup, #accountPopup ").hide("fast");
            });
            //--------------------------global end



            jQuery_11("#datepicker").datepicker({
                showOn: "button",
                buttonImage: "assets/images/calendar.png",
                buttonImageOnly: true
            });



            jQuery_11("#Select_Source").abcDropDown({
                ddwidth: '100%'

            });
            jQuery_11("#division").abcDropDown({
                ddwidth: '100%'

            });
            jQuery_11("#Salesperson").abcDropDown({
                ddwidth: '100%'

            });
            jQuery_11("#Status").abcDropDown({
                ddwidth: '100%'

            });
            jQuery_11("#territory").abcDropDown({
                ddwidth: '100%'

            });







            jQuery_11('.scrollbox').enscroll({
                showOnHover: true,
                verticalTrackClass: 'track3',
                verticalHandleClass: 'handle3'
            });

            jQuery_11('#myNotes').enscroll({
                showOnHover: true,
                verticalTrackClass: 'track3',
                verticalHandleClass: 'handle3'
            });

        
            data = {
                "customer": [
                    {
                        "Company Name": "My Programmer",
                        "Title": "Customer",
                        "First Name": "Joel",
                        "Last Name": "Bills",
                        "Contact Name": "Mieckal",
                        "Work Phone": "01525862",
                        "Home Phone": "98521479",
                        "Fax": "99925852",
                        "Upload File": "image.jpg",
                        "Address Type": "parmanent",
                        "Billing Address": "999 Ansari USA",
                        "City": "Newyork",
                        "State": "USA",
                        "Zip": "100249",
                        "Email": "Joel@gmail.com"
                    },

                {
                    "Company Name": "Janpact",
                    "Title": "Inventory",
                    "First Name": "Poul",
                    "Last Name": "Joseph",
                    "Contact Name": "Mr John",
                    "Work Phone": "95842569",
                    "Home Phone": "85347",
                    "Fax": "99925852",
                    "Upload File": "image.jpg",
                    "Address Type": "pemporary",
                    "Billing Address": "Building 1 USA",
                    "City": "Newyork",
                    "State": "USA",
                    "Zip": "100249",
                    "Email": "poul@gmail.in"
                },
                {
                    "Company Name": "Q-5 Technology",
                    "Title": "Solution",
                    "First Name": "Duck",
                    "Last Name": "Win",
                    "Contact Name": "Mieckal",
                    "Work Phone": "058462",
                    "Home Phone": "915879",
                    "Fax": "854752",
                    "Upload File": "image.jpg",
                    "Address Type": "parmanent",
                    "Billing Address": "999 Ansari USA",
                    "City": "Newyork",
                    "State": "USA",
                    "Zip": "100249",
                    "Email": "duck@gmail.com"
                }

                ]
            };

            var b = 1;
            for (var i = 0; i < data.customer.length; i++) {
                var opp = data.customer[i];
                jQuery_11("<div id='opportunitiesRow_" + b + "'  class='oppRow'><div class='inventoryColumn1'><a href='#' class='inventoryColumn_linkBtn'>" + opp['First Name'] + ' ' + opp['Last Name'] + "</a></div><div class='inventoryColumn2'><a href='#' class='inventoryColumn_linkBtn'>" + opp['Contact Name'] + " </a></div><div class='inventoryColumn3'>" + opp['Work Phone'] + "</div><div class='inventoryColumn4'>" + opp['Email'] + "</div><div class='inventoryColumn5'><span id='myval'>" + opp['State'] + "</span><input type='text' class='inpTextInv' /></div><div class='inventoryColumn6'><a href='#' id='edit_" + b + "' class='edit_class inventoryColumn_linkBtn' onclick='view_class(this.id)'> View </a></div><div class='inventoryColumn7'><a id='delete_" + b + "' href='#' class='delete_class inventoryColumn_linkBtn' onclick='delete_class(this.id)'></a></div></div>").appendTo(jQuery_11("#oppRowGridHolder"));
                b++;
            }

            jQuery_11("#save_Opp").click(function () {
                var Company_Name = jQuery_11("#Company_Name").val();
                var Title = jQuery_11("#Title").val();
                var First_Name = jQuery_11("#First_Name").val();
                var Last_Name = jQuery_11("#Last_Name").val();
                var Contact_Name = jQuery_11("#Contact_Name").val();
                var Work_Phone = jQuery_11("#Work_Phone").val();
                var Home_Phone = jQuery_11("#Home_Phone").val();
                var Fax = jQuery_11("#Fax").val();
                var Upload_File = jQuery_11("#Upload_File").val();
                var Address_Type = jQuery_11("#Address_Type").text();
                var Billing_Address = jQuery_11("#Textarea1").val();
                var City = jQuery_11("#City").val();
                var State = jQuery_11("#State").val();
                var Zip = jQuery_11("#Zip").val();
                var Email = jQuery_11("#Email").val();




                // data.opportunities.push('{"Customer Name":' + '"' + Customer_Name + '"' + ',""Address": ' + '"' + Address + '"' + ',""Contact Name":' + '"' + Contact_Name + '"' + ',"Phone #":' + '"' + Phone + '"' + ',"Company URL":' + '"' + Company_URL + '"' + ',"Name":' + '"' + Name + '"' + ',"Address 1":' + '"' + Address1 + '"' + ',"Address 2":' + '"' + Address2 + '"' + ',"Lead Source":' + '"' + Lead_Source + '"' + ',"Assigned to":' + '"' + Assigned_to + '"' + ',"Appointment Date":' + '"' + datepicker + '"' + ',"Roof Type":' + '"' + Roof_Type + '"' + ',"# of Stories":' + '"' + Stories + '"' + ',"Age":' + '"' + Age + '"' + ',"Notes":' + '"' + Notes + '"' + '}');
                //  data[opportunities] = { "Customer Name": Customer_Name, "Address": Address };
                var obj = new Object();
                obj['Company Name'] = Company_Name;
                obj['Title'] = Title;
                obj['First Name'] = First_Name;
                obj['Last Name'] = Last_Name;
                obj['Contact Name'] = Contact_Name;
                obj['Work Phone'] = Work_Phone;
                obj['Home Phone'] = Home_Phone;
                obj['Fax'] = Fax;
                obj['Upload File'] = Upload_File;
                obj['Address Type'] = Address_Type;
                obj['Billing Address'] = Billing_Address;
                obj['City'] = City;
                obj['State'] = State;
                obj['Zip'] = Zip;
                obj['Email'] = Email;
                data.customer.push(obj);


                var opp = data.customer[b - 1];
                jQuery_11("<div id='opportunitiesRow_" + b + "'  class='oppRow'><div class='inventoryColumn1'><a href='#' class='inventoryColumn_linkBtn'>" + opp['First Name'] + ' ' + opp['Last Name'] + "</a></div><div class='inventoryColumn2'><a href='#' class='inventoryColumn_linkBtn'>" + opp['Contact Name'] + " </a></div><div class='inventoryColumn3'>" + opp['Work Phone'] + "</div><div class='inventoryColumn4'>" + opp['Email'] + "</div><div class='inventoryColumn5'><span id='myval'>" + opp['State'] + "</span><input type='text' class='inpTextInv' /></div><div class='inventoryColumn6'><a href='#' id='edit_" + b + "' class='edit_class inventoryColumn_linkBtn' onclick='view_class(this.id)'> View </a></div><div class='inventoryColumn7'><a id='delete_" + b + "' href='#' class='delete_class inventoryColumn_linkBtn' onclick='delete_class(this.id)'></a></div></div>").appendTo(jQuery_11("#oppRowGridHolder"));
                b++;

            });



            jQuery_11('.scrollbox').enscroll({
                showOnHover: true,
                verticalTrackClass: 'track3',
                verticalHandleClass: 'handle3'
            });



            
            


            //-----
        });

        function delete_class(id) {
            var myId = id;
            var indexIng = jQuery_11("#" + myId).parent().parent().attr("data-index")
            var actVal = jQuery_11("#" + myId).parent().prev().find("input").val();
            jQuery_11("#" + myId).parent().parent().remove();
            //alert();
        };


        function view_class(id) {

            var myId = id;
            var mainId = jQuery_11("#" + myId).parent().parent().attr("id");
            var getId = mainId.split("_");
            var showId = getId[1];
            var test = showId - 1;
            var opp = data.customer[test];
            jQuery_11("#Company_Name").val(opp["Company Name"]);
            jQuery_11("#Title").val(opp["Title"]);
            jQuery_11("#First_Name").val(opp["First Name"]);
            jQuery_11("#Last_Name").val(opp["Last Name"]);
            jQuery_11("#Contact_Name").val(opp["Contact Name"]);
            jQuery_11("#Work_Phone").val(opp["Work Phone"]);
            jQuery_11("#Home_Phone").val(opp["Home Phone"]);
            jQuery_11("#Fax").val(opp["Fax"]);
            jQuery_11("#Upload_File").val(opp["Upload File"]);
            jQuery_11("#Address_Type").text(opp["Address Type"]);
            jQuery_11("#Textarea1").val(opp["Billing Address"]);
            jQuery_11("#City").val(opp["City"]);
            jQuery_11("#State").val(opp["State"]);
            jQuery_11("#Zip").val(opp["Zip"]);
            jQuery_11("#Email").val(opp["Email"]);

            
        });
     /*   function openSearch(id, opener) {
            //alert(opener);
            var ele = document.getElementById(id);
            var top = 0;
            var left = 0;
            var ele = document.getElementById(id);

            while (ele.tagName != "BODY") {
                top += ele.offsetTop;
                left += ele.offsetLeft;
                ele = ele.offsetParent;
            }
            currentlyOpendPopup = opener;
            currentlyClickedBtn = id;
            document.getElementById(opener).style.display = "block";
            document.getElementById(opener).style.top = (top + 10) + "px";
            document.getElementById(opener).style.left = (left - 310) + "px";
        }
       

        */

