

Index.cshtml template:


```
@{
    ViewData["Title] = "Index";
}

<div class="columns col-oneline">
        <div class="column col-6">
            <div class="card-header">
                <div class="card-title h5 text-primary">{{data.entity_label_plural}}</div>
                <div class="card-subtitle text-gray">Manage</div>
            </div>
        </div>
        <div class="column col-6">
            <a id="btnAddData" class="btn float-right">
                <i class="icon icon-plus"></i>
            </a>
        </div>
</div>
<div class="divider"></div>
<div class="columns">
    <div class="column col-6 col-mx-auto">
        <div class="input-group"><span class="input-group-addon">search:</span>
            <input name="q" id="q" class="form-input" type="search" placeholder="term (min. 3 characters)" onkeyup="searchItems();" autocomplete="off">
            <button class="btn btn-primary input-group-btn" onclick="clearSearch();">X</button>
        </div>
        <p class="form-input-hint text-center text-tiny">Upto {{data.search_page_size}} matches returned.</p>
    </div>
</div>

<table class="table mt-4">
    <thead>
        <tr>
    {%-for item in data.attrs%}
            <th>
                {{item.label}}
            </th>
    {%endfor-%}
            <th>
                Actions
            </th>
        </tr>
    <thead>
    <tbody id="listItemHolder">
        <td colspan="5">
            <div class="text-center empty">
                <progress class="progress" max="100"></progress>
                <span class="badge" data-badge="">loading</span>
            </div>

        </td>
    </tbody>
</table>

<div class="container">
    <div class="columns text-center col-gapless mt-8" id="paginationHolder">
    
    
    </div>

</div>

<div class="modal" id="modalForm" data-form-mode="create">
    <a onclick="toggleDataForm();" class="modal-overlay" aria-label="Close"></a>
    <div class="modal-container">
        <div class="modal-header">
            <a onclick="toggleDataForm();" class="btn btn-clear float-right" aria-label="Close"></a>
            <div class="modal-title h5">Add Program</div>
        </div>
        <div class="modal-body">
            <div class="content">
                <form method="post" name="frmData" id="frmData" onsubmit="return false;">
                            @Html.AntiForgeryToken()
                    {%-for item in data.attrs%}
                            <div class="form-group">
                                <label for="{{item.name}}" class="form-label">{{ item.label }}</label>
                                {%-if "referenced_from" in item%}
                                <select for="{{ item.name }}" class="form-select" id="{{ item.name }}" name="{{ item.name }}">
                                </select>
                                {%else%}
                                    {# START: Non-foreign keys #}
                                    {%-if item.attr_type == "string" and item.is_file == False %}
                                    {%-if "big_in_ui" not in item %}
                                <input type="text" class="form-input" minlength="{{item.min}}" maxlength="{{item.max}}" id="{{item.name}}" name="{{item.name}}" />
                                    {%-else-%}
                                <textarea for="{{item.name}}" class="form-input" rows="4" id="{{item.name}}" name="{{item.name}}"></textarea>
                                    {%endif-%}
                                    {%endif-%}
                                    {%if item.attr_type == "int"%}
                                <input type="number" class="form-input" min="{{item.min}}" max="{{item.max}}" id="{{item.name}}" name="{{item.name}}" />
                                    {%endif-%}
                                    {%if item.is_file == True%}
                                <input type="file" class="form-input" id="{{item.name}}" name="{{item.name}}" />
                                    {%endif-%}
                                    {# END: Non-foreign keys #}
                                {%endif-%}
                                    <span id="{{item.name}}" class="text-danger"></span>
                            </div>
                    {%endfor%}
                    <div class="form-group form-check">
                        <label class="form-check-label">
                            <input type="checkbox" class="form-check-input" id="IsActive" name="IsActive" value="true" checked /> Is Active
                        </label>
                    </div>
                    <div class="form-group">
                        <button class="btn btn-primary" id="btnAdd" name="btnAdd">Submit</button>
                    </div>
                </form>
            </div>
        </div>
        <div class="modal-footer">
        </div>
    </div>
</div>

<div class="modal" id="detailsModal">
    <a onclick="toggleDetailsModal();" class="modal-overlay" aria-label="Close"></a>
    <div class="modal-container p-0" id="detailsContent">
    </div>
</div>


<div class="toastr hide" id="toastHolder">

</div>

<script type="text/javascript">
// Helpers
function iget(id){
    return document.getElementById(id);
}
// End Helpers


// UI Tracking
var listMode = 'list';
var showingForm = false;
var showingToast = false;
var showingDetails = false;
var selected = 0;
function toggleDataForm(){
    var elem = iget('modalForm');
    if (showingForm){
        elem.className = 'modal';
        showingForm = false;

    }else{
        elem.className = 'modal active';
        showingForm = true;
    }
    console.log(elem.getAttribute('data-form-mode'));
}

function toggleDetailsModal(item){
    var detailsTemplate = '';
    var elem = iget('detailsModal');
    if (showingDetails){
        elem.className = 'modal active';
        showingDetails = false;
    }else{
        if(item){
            detailsTemplate = `<div class="card">
                <div class="card-header">
                <div class="card-title">Details</div>
            {%-for item in data.attrs%}
                {%if item.show_in_details == True -%}
                {%if item.is_detail_title == True -%}
                {%if 'referenced_from' in item-%}
                    {%if 'referenced_label' in item-%}
                <div class="card-subtitle text-gray">{{item.label}} : <strong>{{'${item.'}}{{lcase(item.referenced_label)}}{{'.'}}{{lcase(item.referenced_label)}}{{'}'}}</strong></div>
                    {%endif-%}
                {%else-%}
                <div class="card-subtitle text-grey">{{item.label}} : <strong>{{'${item.'}}{{lcase(item.name)}}{{'}'}}</strong></div>
                {%-endif-%}
                {%else-%}
                {%if 'referenced_from' in item-%}
                    {%if 'referenced_label' in item-%}
                <div class="card-body">{{item.label}} : {{'${item.}'}}{{lcase(item.referenced_from)}}{{'.'}}{{lcase(item.referenced_label)}}{{'}'}}</div>
                {%endif-%}
                {%else-%}
                <div class="card-body">{{item.label}} : {{'${item.'}}{{lcase(item.name)}}{{'}'}}</div>
                {%-endif-%}
                {%endif-%}
                {%endif-%}
            {%endfor%}
                <div class="card-footer"><button class="btn btn-primary" onclick="toggleDetailsModal();">OK</button></div>
            </div>`;
                var holder = iget('detailsContent');
                holder.innerHTML = detailsTemplate;
            }
            elem.className = 'modal active';
            showingDetails = true;
    }
}

function hidePagination(){
    listMode = 'search';
    iget('paginationHolder').style.display = 'none';
}

function showPagination(){
    listMode = 'list';
    iget('paginationHolder').style.display = 'block';
}

function clearSearch(){
    iget('q').value='';
    gotoPage(1);
    showPagination();
}

function showToast(msgType, msg){
    var elem = iget('toastHolder');
    var toastTemplate = '';
    if (msgType){
        toastTemplate = `<div class="toast toast-${msgType}">
            <button class="btn btn-clear float-right" onClick="hideToast();"></button>
            $(msg)
        </div>`;
    }
    elem.innerHTML = toastTemplate;
    elem.className = 'toastr show';
    showingToast = true;
}

function hideToast(){
    var elem = iget('toastHolder');
    if (showingToast){
        elem.className = 'toastr hide';
        showingToast = false;
    }
}

function makeListHolderEmpty(){
    var emptyState = `<td colspan="5">
            <div class="text-center empty">
                <progress class="progress" max="100"></progress>
                <span class="badge" data-badge="">loading</span>
            </div>
        </td>`;
        iget('listItemHolder').innerHTML = emptyState;
}

function gotoPage(pageNumber){
    currentPage = pageNumber;
    renderPageList();
    makeListHolderEmpty();
    getAllItems();
}

// End UI Tracking

// Request Core
function makeRequest(method, url, data, callback, useToken){
    var xhr = new XMLHttpRequest();
    xhr.onreadystatechange = function() {
        if(xhr.readyState === XMLHttpRequest.DONE){
            if (xhr.status === 200){
                callback(xhr.responseText);
            }else{
                showToast('error', `Unable to make request. Check your connection.`);
            }

        }
    }
    xhr.open(method, url, true);
    xhr.setRequestHeader('Content-Type', 'application/json');
    if (useToken){
        xhr.setRequestHeader(
            'RequestVerificationToken',
            document.getElementsByName('__RequestVerificationToken')[0].value
        );
    }
    xhr.send(JSON.stringify(data));
}

function makeFormRequest(method, url, data, callback, useToken){
    var xhr = new XMLHttpRequest();
    xhr.onreadystatechange = function(){
        if (xhr.readyState === XMLHttpRequest.DONE){
            if (xhr.status === 200){
                callback(xhr.responseText);
            }else{
                showToast('error', `Unable to make request. Check your connection.`);
            }
        }
    }
    xhr.open(method, url, true);
    if (useToken){
        xhr.setRequestHeader(
            'RequestVerificationToken',
            document.getElementsByName('__RequestVerificationToken')[0].value
        );
    }
    xhr.send(data);
}
// End Request Core

//Templating
function renderListItem(item){
    var renderedItem = `
        {%-for item in data.attrs%}
        {%if item.show_in_list == True-%}
        {%if 'referenced_from' in item-%}
                    {%if 'referenced_label' in item-%}
        <td>{{'${item.'}}{{lcase(item.referenced_from)}}{{'.'}}{{lcase(item.referenced_label)}}{{'}'}}</td>
                    {%endif-%}
        {%else-%}
            {%if item.attr_type == 'string'-%}
        <td>{{'${item.'}}{{lcase(item.name)}}{{'.substring(0,20)}'}}</td>
            {%else-%}
        <td>{{'${item.'}}{{lcase(item.name)}}{{'}'}}</td>
            {%endif-%}
        {%-endif-%}
        {%endif-%}
        {%endfor%}
        <td>
            {%if data.allow_edit == True%}<button class="btn btn-sm text-light bg-dark" onClick="fillById(${item.{{lcase(data.entity_name)}}Id});">Edit</button> |{%endif%}
            <button class="btn btn-sm btn-primary" onClick="getById(${item.{{lcase(data.entity_name)}}Id});">Details</button> |
            {%if data.allow_delete == True-%}<button class="btn btn-sm text-dark bg-light" onClick="deleteItem(${item.{{lcase(data.entity_name)}}Id},'${item.{{lcase(data.delete_attr_label)}}}');">X</button>{%endif%}
        </td>
    `;
    return renderedItem;
}

var currentPage = 1;
var pageSize = {{ data.list_page_size }};
var totalPageCount = 1;
function renderPagingList(){
    totalPageCount = Math.ceil(listCount / pageSize);
    var pageLimit = 0;
    var startPage = 0;

    //Extra Logic
    if (currentPage >= 2){
        startPage = currentPage - 1;
        if (totalPageCount <= (currentPage + 1)){
            pageLimit = totalPageCount;
        }else{
            pageLimit = currentPage + 1;
        }
    }else{
        startPage = 1;
        if (totalPageCount < 3){
            pageLimit = totalPageCount;
        }else{
            pageLimit = 3;
        }
    }

    var pageButtons = '';
    var activeSwitch = '';
    for (let i = startPage; i <= pageLimit; i++){
        if((i+1) == currentPage){
            activeSwitch = ' active';
        }else{
            activeSwitch = '';
        }
        pageButtons += ` <button class="btn btn-sm" onclick="gotoPage(${i});">${i}</button>`;
    }
    var content = `<div class="column col-12 text-center">${pageButtons}</div>`;
    iget('paginationHolder').innerHTML = content;
}
// End Templating

// Lookups
{%-for item in data.attrs%}
{%if 'referenced_from' in item-%}
{%-if 'referenced_label' in item-%}
function get{{item.alt_plural}}(){{'{'}}
    makeRequest('GET', '/{{data.entity_name_lower}}/{{lcase(item.alt_plural)}}', null, function(response){
        var items = JSON.parse(response);
        var itemList = iget('{{item.name}}');
        itemList.innerHTML = '';
        items.result.forEach(function(item) {
            var elem = document.createElement('option');
            var template = `{{'${item.'}}{{lcase(item.referenced_label)}}{{'}'}}`;
            elem.innerHTML = template;
            elem.value = item.{{lcase(item.name)}};
            itemList.appendChild(elem);
        });
    });
{{'}'}}
{%-endif-%}
{%-endif-%}
{%-endfor-%}
// End Lookups

// Data Calls
var listCount = 0;

function getCount(){
    makeRequest('GET', '/{{data.entity_name_lower}}/count', null, function(response){
        listCount = parseInt(response);
        renderPagingList();
    });
}

function getAllItems() {
    setTimeout(function(){
        var skip = (currentPage - 1) * pageSize;
        var getAllUrl = '/{{data.entity_name_lower}}/all?skip=' + skip;
        makeRequest('GET', getAllUrl, null, function(response){
            var items = JSON.parse(response);
            var itemsList = iget('listItemHolder');
            itemsList.innerHTML = '';
            items.result.forEach(function(item){
            var elem = document.createElement('tr');
            elem.id = 'item' + item.{{lcase(data.entity_name)}}Id;
            var template = renderListItem(item);
            elem.innerHTML = template;
            itemsList.appendChild(elem);
            });
        });
    }, 1500);
}

function searchItems(){
    var term = iget('q').value;
    if (term.length>2){
        hidePagination();
        makeRequest('GET', '/{{data.entity_name_lower}}/search?term=' + term, null, function(response){
            var items = JSON.parse(response);
            var itemsList = iget('listItemsHolder');
            itemsList.innerHTML = '';
            items.result.forEach(function(item){
                var elem = document.createElement('tr');
                elem.id = 'item' + item.{{lcase(data.entity_name)}}Id;
                var template = renderListItem(item);
                elem.innerHTML = template;
                itemsList.appendChild(elem);
            });
        });
    }
    if (term.length == 0){
        gotoPage(1);
        showPagination();
    }
}

function addItem(){
    var formData = new FormData();
    {%-for item in data.attrs%}
    {%-if item.show_in_create == True%}
    var {{lcase(item.name)}} = iget('{{item.name}}').value;
    {%-endif-%}
    {%-endfor%}
    var isActive = iget('IsActive').value;
    {%-for item in data.attrs%}
    {%-if item.show_in_create == True%}
    formData.append("{{item.name}}", {{lcase(item.name)}});
    {%-endif-%}
    {%-endfor%}
    formData.append("IsActive", isActive);
    makeFormRequest('POST', '/{{data.entity_name_lower}}/create',
        formData,
        function(response){
            var item = JSON.parse(response);
            toggleDataForm();
            listCount += 1;
            showingToast('primary', `Added "{item.value.{{lcase(data.create_attr_label)}}}".`);
            renderPagingList();
{%-if data.order_by == "desc"%}
            if (currentPage == 1)
{%-else%}
            if (currentPage == totalPageCount)
{%-endif%}
        {
            // Lookup ref.
{%-for item in data.attrs-%}
{%if 'referenced_from' in item-%}
{%-if 'referenced_label' in item%}
        item.value.{{lcase(item.referenced_from)}} = {};
        var {{lcase(item.referenced_from)}}{{item.referenced_label}} = iget('{{item.name}}').innerHTML;
        item.value{{lcase(item.referenced_from)}}.{{lcase(item.referenced_label)}} = {{lcase(item.referenced_from)}}{{item.referenced_label}};
{%-endif-%}
{%-endif-%}
{%-endfor%}
        var template = renderListItem(item.value);
        var elem = document.createElement('tr');
        elem.id = 'item' + item.value.{{lcase(data.entity_name)}}Id;
        elem.innerHTML = template;
        var itemsList = iget('listItemsHolder');
{% if data.order_by == 'desc'%}
        itemsList.removeChild(itemsList.lastChild);
        itemsList.insertBefore(elem, itemsList.firstChild);
{%else%}
        itemsList.append(elem);
{%endif%}
        }
        iget("frmData").reset();
        }, true);
}

{%if data.allow_delete == True-%}
function deleteItem(id, {{lcase(data.delete_attr_label)}}) {
    makeRequest('POST', '/{{data.entity_name_lower}}/delete/' + id, {id:id}, function(response){
        const element = iget('item' + id);
        element.remove();
        listCount -= 1;
        totalPageCount = Math.ceil(listCount / pageSize);
        if (currentPage > totalPageCount){
            currentPage = totalPageCount;
            gotoPage(currentPage);
        }
        showToast('error', `Removed "${title}".`);
        renderPagingList();
    }, true);
}
{%endif%}

function getById(id){
    makeRequest('GET','/{{data.entity_name_lower}}/'+id, {id : id}, function(response){
        var item = JSON.parse(response);
        toggleDetailsModal(item);
    }, false);
}

function fillById(id){
    selected = id;
    makeRequest('GET', '/{{data.entity_name_lower}}/' + id, {id : id}, function(response){
        toggleDataForm();
        var item = JSON.parse(response);
{%-for item in data.attrs%}
{%-if item.show_in_edit == True%}
    iget('{{item.name}}').value = item.{{lcase(item.name)}};
{%-endif-%}
{%-endfor%}
    iget('frmData').setAttribute('action', '{{data.entity_name_lower}}/edit/' + item.{{lcase(data.entity_name)}}Id);
    iget('modalForm').setAttribute('data-form-mode', 'edit');    
    }, false);
}

{%if data.allow_edit == True%}
function editItem(){
    var formData = new FormData();
    {%-for item in data.attrs%}
    {%-if item.show_in_edit == True%}
        var {{lcase(item.name)}} = iget('{{item.name}}').value;
    {%-endif-%}
    {%-endfor%}
    var isActive = iget('IsActive').value;
    formData.append("{{data.entity_name}}Id", selected);

{%-for item in data.attrs%}
{%-if item.show_in_edit == True%}
    formData.append("{{item.name}}", {{lcase(item.name)}});
{%-endif-%}
{%-endfor%}
    formData.append("IsActive", isActive);
    makeFormRequest('POST', iget('frmData').getAttribute('action'),
        formData,
        function(response) {
            toggleDataForm();
            var item = JSON.parse(response);
            var template = renderListItem(item.result);
            iget('item' + item.result.{{lcase(data.entity_name)}}Id).innerHTML = template;
            showToast('primary', `Updated "${iget('{{data.edit_attr_label}}').value}".`);
        }, true);
}
{%endif%}
// End Data Calls

// Event Binding
window.onload = function init(){
    iget('btnAddData').addEventListener('click', function(){
        iget('frmData').setAttribute('action', '{{data.entity_name_lower}}/create');
        iget('modalForm').setAttribute('data-form-mode','create');
        iget("frmData").reset();
        toggleDataForm();
    });
    iget('btnAdd').addEventListener('click', function(){
        var elem = iget('modalForm');
        var mode = elem.getAttribute('data-form-mode');
        if (mode=='create'){
            addItem();
        }else{
            editItem();
        }
    });
    getCount();
{%-for item in data.attrs-%}
{%if 'referenced_from' in item-%}
{%-if 'referenced_label' in item%}
    get{{item.alt_plural}}();
{%-endif-%}
{%-endif-%}
{%-endfor%}
    getAllItems();
    iget('q').focus();
}
// End Event Binding
</script>
```

Please convert the below Jinja template to liquid template.

```jinja
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace {{ data.app_name }}.Models;

public class {{data.entity_name}}
{

    [Key]
    public int {{ data.entity_name}}Id {get;set;}
    {%for item in data.attrs%}
    {%-if item.attr_type == "datetime"-%}
        {%-set a_type="Datetime"-%}
    {%-else-%}
        {%-set a_type=item.attr_type-%}
    {%-endif-%}
    {% if item.isrequired == True%}
    [Required]
    {% endif-%}
    {%if "label" in item%}[Display(Name = "{{ item.label}}")]{%endif%}
    {%if item.attr_type == "string"%}[StringLength({{item.max}}, MinimumLength = {{item.min}})]{%endif-%}
    {%if item.attr_type == "int"%}{%if "min" in item%}[MinValue({{item.min}})]{%endif%}{%endif-%}
    {%if item.attr_type == "int"%}{%if "max" in item%}[MaxValue({{item.max}})]{%endif%}{%endif-%}
    {%if item.attr_type == "datetime"%}[DataType(DataType.Data)]{%endif%}
    {%if item.isforeign == True%}[ForeignKey("{{item.name}}")]{%endif%}
    public {{ a_type}} {{item.name}} { get;set; }
    {%if "default" in item-%}
    {%if item.attr_type == "string"-%} = "{{ item.default }}";{%-else-%} = {{ item.default }};{%-endif-%}{%-endif%}
    {%-if "referenced_from" in item%}
    {%if "label" in item%}
    [Display(Name = "{{ item.label }}")]
    {%endif%}
    public virtual {{item.referenced_from}}? {{item.referenced_from}} {get;set;}
    {%endif%}
    {%-endfor%}

    {%for item in data.referenced_by_entities-%}
    public virtual List<{{item.name}}> {{item.name_plural}} {get;set} = new List<{{item.name}}>();
    {%-endfor%}
    public int CreatedBy {get;set;}
    [DataType(DataType.Date)]
    public DateTime? CreatedOn {get;set;} = DateTime.Now;
    public int ModifiedBy {get;set;}
    [DataType(DataType.Date)]
    public DateTime? LastModifiedOn { get;set; } = DateTime.Now;
    [Display(Name="Is Active")]
    public Boolean IsActive {get; set;} = true;
}
```