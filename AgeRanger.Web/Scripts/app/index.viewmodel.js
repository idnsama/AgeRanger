function ListViewModel() {
    var self = this;

    //Holds data (item list) for the current page.
    self.CurrentPageData = ko.observable();
    //This variable contains customer's input for the filter field.
    self.FilterStringInput = ko.observable("");
    //This variable contains active filter. Empty string filter would show all available records (paginated).
    self.FilterStringCurrent = ko.observable("");
    //Position at the top of the page. 0 is the default position for paging.
    self.CurrentPage = ko.observable(0);
    //Default size of the page.
    self.PageSize = ko.observable(10);
    //Total count of the whole list.
    self.TotalCount = ko.observable(0);

    self.TotalPages = ko.computed(function () {
        return Math.ceil(self.TotalCount() / self.PageSize());
    });

    //Calculates position of the first item on the page based on the lenght of the full list.
    self.StartPosition = ko.computed(function () {
        return self.CurrentPage() * self.PageSize();
    });
    //True if current page isn't the last one.
    self.CanGoToTheNextPage = ko.computed(function () {
        if ((self.CurrentPage() + 1) * self.PageSize() >= self.TotalCount())
            return false;
        else
            return true;
    });


    //On Search click/enter.
    self.ApplyFilter = function () {
        self.FilterStringCurrent(self.FilterStringInput());
        self.UpdateTotal();
        self.GoToPage(0);
    };

    //On Clear click.
    self.ClearFilter = function () {
        self.FilterStringCurrent("");
        self.FilterStringInput("");
        self.UpdateTotal();
        self.GoToPage(0);
    };


    //Jump to page number, with regards to the filter.
    self.GoToPage = function (pageNumber) {
        self.CurrentPage(pageNumber);
        $.ajax({
            url: '/api/getFilteredList',
            type: 'get',
            data: { position: self.StartPosition(), length: self.PageSize(), filterString: self.FilterStringCurrent() },
            contentType: 'application/json',
            success: function (data) {
                self.CurrentPageData(data);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert("Server error: \"" + errorThrown + "\".");
            }
        });
    };

    //Update total items counter from DB with regards to the filter.
    self.UpdateTotal = function () {
        $.ajax({
            url: '/api/getTotalCount',
            type: 'get',
            data: { filterString: self.FilterStringCurrent() },
            contentType: 'application/json',
            success: function (data) {
                self.TotalCount(data);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert("Server error: \"" + errorThrown + "\".");
            }
        });
    };

    //Loads next page.
    self.GoToNextPage = function () {
        self.GoToPage(self.CurrentPage() + 1);
    };
    //Loads previous page.
    self.GoToPreviousPage = function () {
        if (self.CurrentPage() > 0) self.GoToPage(self.CurrentPage() - 1);
    };

    //Initial page load.
    self.GoToPage(0);
    //Initial total count load.
    self.UpdateTotal(); 





    //Holds data for the person selected for editing.
    self.CurrentPersonData = new ItemViewModel();

    // Update click.
    self.UpdatePerson = function (Person) {
        self.CurrentPersonData.LoadData(Person);
        $('#myModal').modal('show');
    }

    // Closes modal.
    self.ModalCancel = function () {
        $('#myModal').modal('hide');
    }

    //Combined Add/Update function. Id from item submodel is used to select proper operation.
    self.ModalSave = function () {
        //Validate before calling API.
        if (!self.CurrentPersonData.validator.isValid())
        {
            self.CurrentPersonData.validator.errors.showAllMessages(true);
            return;
        }

        if (self.CurrentPersonData.id && self.CurrentPersonData.id() > 0) {
            $.ajax({
                url: '/api/postUpdatePerson',
                type: 'post',
                data: ko.toJSON(self.CurrentPersonData),
                contentType: 'application/json',
                success: function (data) {
                    if (data) {
                        self.GoToPage(self.CurrentPage());
                        $('#myModal').modal('hide');
                    }
                    else {
                        alert("Person record was not updated.");
                    }
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    alert("Server error: \"" + errorThrown + "\".");
                },
                complete: function () {
                }

            });
        }
        else {
            $.ajax({
                url: '/api/postAddPerson',
                type: 'post',
                data: ko.toJSON(self.CurrentPersonData),
                contentType: 'application/json',
                success: function (data) {
                    if (data) {
                        self.UpdateTotal();
                        self.GoToPage(0);
                        $('#myModal').modal('hide');
                    }
                    else {
                        alert("Person record was not added.");
                    }
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    alert("Server error: \"" + errorThrown + "\".");
                },
                complete: function () {
                }
            });
        }
    }

    self.DeletePerson = function (Person) {
        $.ajax({
            url: '/api/getDeletePerson',
            type: 'get',
            data: { id: Person.id },
            contentType: 'application/json',
            success: function (data) {
                if (data) {
                    self.UpdateTotal();
                    self.GoToPage(self.CurrentPage());
                }
                else {
                    alert("Error deleting a person record.");
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert("Server error: \"" + errorThrown + "\".");
            },
            complete: function () {
                $('#myModal').modal('hide');
            }
        });
    }

    self.AddPerson = function () {
        self.CurrentPersonData.LoadData({ id: 0, firstName: "", lastName: "", age: "" });
        $('#myModal').modal('show');
    }
}

//Person editing model with validation.
function ItemViewModel() {
    var self = this;

    self.id = ko.observable();

    self.validator = ko.validatedObservable([
        self.firstName = ko.observable("").extend({ required: true, minLength: 1 }),
        self.lastName = ko.observable("").extend({ required: true, minLength: 1 }),
        self.age = ko.observable("").extend({ required: true, min: 0, digit: true })
    ]);

    //Copy data to the model by value.
    self.LoadData = function (Item) {
        self.id(Item.id);
        self.firstName(Item.firstName);
        self.lastName(Item.lastName);
        self.age(Item.age);
    };
}

var pageModel = new ListViewModel();

//Initializing...
$(function () {
    ko.validation.rules.pattern.message = 'Invalid.';
    ko.validation.init({
        registerExtenders: true,
        messagesOnModified: true,
        insertMessages: true,
    });

    ko.applyBindings(pageModel);
});
