function getQueryString(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
    var r = window.location.search.substr(1).match(reg);
    if (r != null) return unescape(r[2]); return null;
}
//修改URL参数
function changeURLPar(destiny, par, par_value) {

    var pattern = par + '=([^&]*)';
    var replaceText = par + '=' + par_value;
    if (destiny.match(pattern)) {
        var tmp = '/\\' + par + '=[^&]*/';
        tmp = destiny.replace(eval(tmp), replaceText);
        return (tmp);
    }
    else {
        if (destiny.match('[\?]')) {
            return destiny + '&' + replaceText;
        }
        else {
            return destiny + '?' + replaceText;
        }
    }
    return destiny + '\n' + par + '\n' + par_value;
}
//跳转到指定URL
function ToURL(url) {
    window.location.href =getRootPath()+ url;
}
//新标签页打开指定URL
function OpenURL(url) {
    window.open(url);
}
//跳转到第几页
function ToPage(pageIndex) {
    var url = changeURLPar(location.href, "pageindex", pageIndex);
    window.location.href = url;
}
//重新排序
function reOrder(order) {
    var url = changeURLPar(location.href, "order", order);

    window.location.href = url;
}
//下一页
function toNextPage() {
    var pageIndex = parseInt(getQueryString("pageindex")) + 1;
    var url = changeURLPar(location.href, "pageindex", pageIndex);
    window.location.href = url;
}
//上一页
function toLastPage() {
    var pageIndex = parseInt(getQueryString("pageindex")) - 1;
    var url = changeURLPar(location.href, "pageindex", pageIndex);
    window.location.href = url;
}
function getCookie(cookie_name) {
    var allcookies = document.cookie;
    var cookie_pos = allcookies.indexOf(cookie_name);
    if (cookie_pos != -1) {
        // 把cookie_pos放在值的开始，只要给值加1即可。
        cookie_pos += cookie_name.length + 1;
        var cookie_end = allcookies.indexOf(";", cookie_pos);
        if (cookie_end == -1) {
            cookie_end = allcookies.length;
        }
        var value = unescape(allcookies.substring(cookie_pos, cookie_end));
    }
    return value;
}
function getTimeSpan() {
    return new Date().getTime();
}

//获取根目录
function getRootPath() {
    //获取当前网址，如： http://localhost:8083/uimcardprj/share/meun.jsp
    var curWwwPath = window.document.location.href;
    //获取主机地址之后的目录，如： uimcardprj/share/meun.jsp
    var pathName = window.document.location.pathname;
    var pos = curWwwPath.indexOf(pathName);
    //获取主机地址，如： http://localhost:8083
    var localhostPaht = curWwwPath.substring(0, pos);
    //获取带"/"的项目名，如：/uimcardprj
    //var projectName = pathName.substring(0, pathName.substr(1).indexOf('/') + 1);
    return (localhostPaht);
}