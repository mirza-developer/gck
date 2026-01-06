window.initFooterMap = function () {
    if (window.footerMapInitialized) return;
    window.footerMapInitialized = true;
    var mapDiv = document.getElementById('footer-map');
    if (!mapDiv) return;
    var map = new L.Map("footer-map", {
        key: "web.d9e4389cc2e54a848a7c83d14a8535ec",
        maptype: "neshan",
        poi: false,
        traffic: false,
        center: [35.6837518, 51.3858216],
        zoom: 14,
    });
    let marker = L.marker([35.6837518, 51.3858216]).addTo(map);
};
