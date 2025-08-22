/*
 * Library: FooPicker
 * Description: Pure JavaScript date picker
 * Author: Yogasaikrishna
 * License: MIT
 * URL: https://github.com/yogasaikrishna/foopicker
 */

var FooPicker = (function () {
  'use strict';

  var hasEventListener = window.addEventListener;
  var weeks = ['L', 'M', 'M', 'J', 'V', 'S', 'D'];

  function FooPicker() {
    var _self = this;
    var _id;

    var defaults = {
      className: 'foopicker',
      dateFormat: 'dd/MM/yyyy'
    };

    if (arguments[0] && typeof arguments[0] === "object") {
      _self.options = extendOptions(defaults, arguments[0]);
      _id = _self.options.id;
    }

    // Show date picker on click
    _self.showPicker = function() {
      _self.buildPicker();
      var pickerField = document.getElementById(_id);
      var pickerDiv = document.getElementById('foopicker-' + _id);
      if (pickerField) {
        var datepicker = pickerField.getBoundingClientRect();
        var left = datepicker.left;
        var top = datepicker.bottom - 7;
        if (pickerDiv) {
          pickerDiv.style.position = 'absolute';
          pickerDiv.style.top = top + 'px';
          pickerDiv.style.left = left + 'px';
        }
      }
    };

    // Hide date picker
    _self.hidePicker = function(event) {
      setTimeout(function() {
        var pickerDiv, pickerField;
        if (!_self.monthChange) {
          _self.removeListeners(_id);
          pickerDiv = document.getElementById('foopicker-' + _id);
          pickerDiv.innerHTML = '';
        } else {
          pickerField = document.getElementById(_self.options.id);
          if (pickerField) {
            pickerField.focus();
          }
          _self.monthChange = false;
        }
      }, 210);
    };

    // Select date
    _self.selectDate = function(event) {-
      _self.monthChange = false;
      var el = document.getElementById(event.target.id);
      var pickerField = document.getElementById(_id);
      if (el) {
        el.classList.add('foopicker__day--selected');
        var date = format(_self, el.dataset.day, el.dataset.month, el.dataset.year);
        _self.selectedDate = date;
        _self.selectedDay = parseInt(el.dataset.day);
        _self.selectedMonth = parseInt(el.dataset.month);
        _self.selectedYear = parseInt(el.dataset.year);
        if (pickerField) {
            var fecha = formatearfecha(date);
            pickerField.value = fecha;
        }
      }
      _self.hidePicker();
    };

    _self.removeListeners = function(id) {
      var picker = document.getElementById(id);
      if (picker) {
        var el = picker.getElementsByClassName('foopicker__day');
        if (el && el.length) {
          for (var count = 0; count < el.length; count++) {
            if (typeof el[count].onclick === "function") {
              var elem = document.getElementById(id + '-foopicker__day--' + (count + 1));
              removeEvent(elem, 'click', _self.selectDate, false);
            }
          }
        }
      }
      document.removeEventListener('keydown', keyDownListener, false);
    };

    _self.changeMonth = function(event) {
      var className = event.target.className, positive = false;
      if (className.indexOf('foopicker__arrow--next') !== -1) {
        positive = true;
      }
      _self.monthChange = true;
      var day = _self.currentDate;
      var month = positive ? _self.currentMonth + 1 : _self.currentMonth - 1;
      var year = _self.currentYear;
      _self.currentMonth = month;
      Calendar.date = new Date(year, month , day);
      var pickerDiv = document.getElementById('foopicker-' + _id);
      if (pickerDiv) {
        var datepicker = pickerDiv.querySelector('.foopicker');
        datepicker.innerHTML = Calendar.buildHeader() + Calendar.buildCalendar();
        Calendar.addListeners(_self);
      }
    };

    _self.buildPicker = function() {
      var pickerDiv = document.getElementById('foopicker-' + _id);
      if (pickerDiv && !hasPicker(pickerDiv)) {
        var fragment, datepicker, calendar;
        fragment = document.createDocumentFragment();
        datepicker = document.createElement('div');
        // Add default class name
        datepicker.className = _self.options.className;

        var date;
        // Date is selected show that month calendar
        if (_self.selectedDate) {
          date = new Date(_self.selectedYear, _self.selectedMonth - 1, _self.selectedDay);
        } else {
          date = new Date();
        }
        Calendar.date = date;

        // Add calendar to datepicker
        datepicker.innerHTML = Calendar.buildHeader() + Calendar.buildCalendar();
        // Append picker to fragment and add fragment to DOM
        fragment.appendChild(datepicker);
        pickerDiv.appendChild(fragment);

        Calendar.addListeners(_self);
      }
      document.addEventListener('keydown', keyDownListener, false);
    };

    _self.buildTemplate = function() {
      var pickerDiv = document.createElement('div');
      pickerDiv.id = 'foopicker-' + _id;
      document.body.appendChild(pickerDiv);
      addListeners(_self);
    };

    _self.destroy = function() {
      var pickerDiv = document.getElementById('foopicker-' + _id);
      if (pickerDiv) {
        document.body.removeChild(pickerDiv);
      }
    };

    function keyDownListener() {
      _self.monthChange = false;
      _self.hidePicker();
    }

    _self.buildTemplate();
  }

  // Date formatter
  function format(instance, day, month, year) {
    switch(instance.options.dateFormat) {
      case 'dd-MM-yyyy':
        return day + '-' + month + '-' + year;
      case 'dd-MMM-yyyy':
        return day + '-' + getShortMonth(month) + '-' + year;
      case 'dd.MM.yyyy':
        return day + '.' + month + '.' + year;
      case 'dd.MMM.yyyy':
        return day + '.' + getShortMonth(month) + '.' + year;
      case 'dd/MM/yyyy':
        return day + '/' + month + '/' + year;
      case 'dd/MMM/yyyy':
        return day + '/' + getShortMonth(month) + '/' + year;
      case 'MM-dd-yyyy':
        return month + '-' + day + '-' + year;
      case 'MM.dd.yyyy':
        return month + '.' + day + '.' + year;
      case 'MM/dd/yyyy':
        return month + '/' + day + '/' + year;
      case 'yyyy-MM-dd':
        return year + '-' + month + '-' + day;
      case 'yyyy-MMM-dd':
        return year + '-' + getShortMonth(month) + '-' + day;
      case 'yyyy.MM.dd':
        return year + '.' + month + '.' + day;
      case 'yyyy.MMM.dd':
        return year + '.' + getShortMonth(month) + '.' + day;
      case 'yyyy/MM/dd':
        return year + '/' + month + '/' + day;
      case 'yyyy/MMM/dd':
        return year + '/' + getShortMonth(month) + '/' + day;
      default:
        return day + '/' + month + '/' + year;
    }
  }

  // Extend default options
  function extendOptions(defaults, options) {
    var property;
    for (property in options) {
      if (options.hasOwnProperty(property)) {
        defaults[property] = options[property];
      }
    }
    return defaults;
  }

  // Build Calendar
  var Calendar = {
    // Get current date
    date: new Date(),

    // Get day of the week
    day: function() {
      return this.date.getDay();
    },

    // Get today day
    today: function() {
      return this.date.getDate();
    },

    // Get current month
    month: function() {
      return this.date.getMonth();
    },

    // Get current year
    year: function() {
      return this.date.getFullYear();
    },

    rowPadding: function() {
      var startWeekDay = getWeekDay(1, this.month(), this.year());
      return [6, 0, 1, 2, 3, 4, 5][startWeekDay];
    },

    // Build calendar header
    buildHeader: function() {
      return '<div class="foopicker__header">' +
        '<div class="foopicker__arrow foopicker__arrow--prev"></div>' +
        '<div class="foopicker__month">' + getMonths(this.month()) +
        '&nbsp;&nbsp;' + this.year() + '</div>' +
        '<div class="foopicker__arrow foopicker__arrow--next"></div></div>';
    },

    // Build calendar body
    buildCalendar: function() {
      var index;
      var daysInMonth = getDaysInMonth(this.year(), this.month());
      var template = '<div class="foopicker__calendar"><table><tr>';
      for (index = 0; index < weeks.length; index++) {
        template += '<td><div class="foopicker__week">' + weeks[index] + '</div></td>';
      }
      template += '</tr><tr>';
      var columnIndex = 0, dayClass = '';
      var day = 0 - this.rowPadding();
      for (; day < daysInMonth; day++) {
        if (day < 0) {
          template += '<td></td>';
        } else {
          // dayClass = day === (this.today() - 1) ? 'foopicker__day--today' : '';
          template += '<td><div class="foopicker__day ' + dayClass + '" ';
          template += 'data-day="' + (day + 1) + '" data-month="' + (this.month() + 1);
          template += '" data-year="' + this.year() + '" ';
          template += '>' + (day + 1) + '</div></td>';
        }
        columnIndex++;
        if (columnIndex % 7 === 0) {
          columnIndex = 0;
          template += '</tr><tr>';
        }
      }
      template += '</tr></table></div>';
      return template;
    },

    // Header click listeners
    addListeners: function(instance) {
      var id = instance.options.id;
      var pickerDiv = document.getElementById('foopicker-' + id);
      if (pickerDiv) {
        var prevBtn = pickerDiv.getElementsByClassName('foopicker__arrow--prev')[0];
        var nextBtn = pickerDiv.getElementsByClassName('foopicker__arrow--next')[0];
        addEvent(prevBtn, 'click', instance.changeMonth, false);
        addEvent(nextBtn, 'click', instance.changeMonth, false);
      }

      this.modifyDateClass(instance);

      var el = pickerDiv.getElementsByClassName('foopicker__day');
      if (el && el.length) {
        for (var count = 0; count < el.length; count++) {
          if (typeof el[count].onclick !== "function") {
            var elem = document.getElementById(id + '-foopicker__day--' + (count + 1));
            addEvent(elem, 'click', instance.selectDate, false);
          }
        }
      }

      this.changeInstanceDate(instance);
    },

    modifyDateClass: function(instance) {
      var id = instance.options.id, day = instance.selectedDay,
        month = instance.selectedMonth - 1, year = instance.selectedYear;
      var pickerDiv = document.getElementById('foopicker-' + id);
      if (pickerDiv) {
        var el = pickerDiv.getElementsByClassName('foopicker__day');
        if (el && el.length) {
          for (var count = 0; count < el.length; count++) {
            if ((count + 1) === day && this.month() === month &&
              this.year() === year) {
              el[count].className = 'foopicker__day foopicker__day--selected';
            } else {
              el[count].className = 'foopicker__day';
            }
            if ((count + 1) === instance.currentDate &&
              this.month() === instance.currentMonth - 1 &&
              this.year() === instance.currentYear) {
              el[count].classList.add('foopicker__day--today');
            }
            el[count].id = id + '-foopicker__day--' + (count + 1);
          }
        }
      }
    },

    // Change date in instance
    changeInstanceDate: function(instance) {
      instance.currentDay = this.day();
      instance.currentDate = this.today();
      instance.currentMonth = this.month();
      instance.currentYear = this.year();
    }
  };

  function addListeners(picker) {
    var el = document.getElementById(picker.options.id);
    if (el) {
      addEvent(el, 'click', picker.showPicker, false);
      addEvent(el, 'blur', picker.hidePicker, false);
    }
  }

  function getMonths(month) {
    var months = ['Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio', 'Julio',
      'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre'];
    return month >= 0 ? months[month] : months;
  }

  function getShortMonth(month) {
    return ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep',
      'Oct', 'Nov', 'Dec'][parseInt(month) - 1];
  }

  function getDaysInMonth(year, month) {
    return [31, isLeapYear(year) ? 29 : 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31][month];
  }

  function getWeekDay(date, month, year) {
    return new Date(year, month, date).getDay();
  }

  // Check if current year is leap year
  function isLeapYear(year) {
    return year % 100 === 0 ? year % 400 === 0 ? true : false : year % 4 === 0;
  }

  // Check if foopicker is already built and added to DOM
  function hasPicker(el) {
    return el && el.querySelector('.foopicker') ? true : false;
  }

  // Function to add events
  function addEvent(el, type, callback, capture) {
    if (hasEventListener) {
      if (el) {
        el.addEventListener(type, callback, capture);
      }
    } else {
      if (el) {
        el.attachEvent('on' + type, callback);
      }
    }
  }

  // Function to remove events
  function removeEvent(el , type, callback, capture) {
    if (hasEventListener) {
      if (el) {
        el.removeEventListener(type, callback, capture);
      }
    } else {
      if (el) {
        el.detachEvent('on' + type, callback);
      }
    }
  }

  return FooPicker;
})();


function formatearfecha(fecha) {
    var campos = fecha.split("/");
    var anio = campos[2];
    var mes = campos[1];
    mes = mes.length > 1 ? mes : '0' + mes;
    var dia = campos[0]
    dia = dia.length > 1 ? dia : '0' + dia;
    return dia + '/' + mes + '/' + anio;
}