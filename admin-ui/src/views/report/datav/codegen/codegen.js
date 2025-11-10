import JSZip from "jszip";
import FileSaver from "file-saver";
import { rptInfo } from "@/api/report/report";

let optionMap = new Map();

export function addOption(bindingDiv, option) {
  optionMap.set(bindingDiv, option);
}

export function deleteOption(bindingDiv) {
  optionMap.delete(bindingDiv);
}

export function getOption() {
  return optionMap;
}

export function emptyOption() {
  optionMap = new Map();
}

export function codeGen(screenId) {
  rptInfo(screenId).then((response) => {
    // console.log(response);
    if (response.code == 200) {
      let zip = new JSZip();

      var htmlStr =
        '<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">\
                    <html xmlns="http://www.w3.org/1999/xhtml">\
                    <head>\
                    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />\
                    <title>' +
        response.data.screenName +
        '</title>\
                    <meta name="viewport" content="width=device-width, initial-scale=1, maximum-scale=1">';

      var cssFolder = zip.folder("css");
      var jsFolder = zip.folder("js");
      var imgFolder = zip.folder("images");

      var cssLinkHtml = "";
      var jsLinkHtml = "";
      var jsfuncHtml = "";
      var bodyHtml = "</head><body>";

      // 字体
      cssFolder.file("Digital.ttf", getBlob("/codegen/css/Digital.ttf"));
      cssFolder.file("Chunkfive.otf", getBlob("/codegen/css/Chunkfive.otf"));
      cssFolder.file("Chunkfive.otf", getBlob("/codegen/css/UnidreamLED.ttf"));
      cssFolder.file("font.css", getBlob("/codegen/css/font.css"));
      cssLinkHtml = cssLinkHtml + '<link rel="stylesheet" href="css/font.css">';

      // 动画
      cssFolder.file("animate.css", getBlob("/codegen/css/animate.css"));
      cssLinkHtml =
        cssLinkHtml + '<link rel="stylesheet" href="css/animate.css">';

      // echarts依赖的JS
      jsFolder.file("echarts.min.js", getBlob("/codegen/js/echarts.min.js"));
      jsLinkHtml =
        jsLinkHtml +
        '<script type="text/javascript" src="js/echarts.min.js"></script>';

      // china.js依赖
      jsFolder.file("china.js", getBlob("/codegen/js/china.js"));
      jsLinkHtml =
        jsLinkHtml +
        '<script type="text/javascript" src="js/china.js"></script>';

      // echarts-liquidfill.min.js依赖
      jsFolder.file(
        "echarts-liquidfill.min.js",
        getBlob("/codegen/js/echarts-liquidfill.min.js")
      );
      jsLinkHtml =
        jsLinkHtml +
        '<script type="text/javascript" src="js/echarts-liquidfill.min.js"></script>';

      // jquery-3.2.1.min.js依赖
      jsFolder.file(
        "jquery-3.2.1.min.js",
        getBlob("/codegen/js/jquery-3.2.1.min.js")
      );
      jsLinkHtml =
        jsLinkHtml +
        '<script type="text/javascript" src="js/jquery-3.2.1.min.js"></script>';

      let drawingList = JSON.parse(response.data.drawOption);
      let themeForm = JSON.parse(response.data.themeOption);
      let optionMap = JSON.parse(response.data.optionMap);
      //主题注册
      jsfuncHtml =
        jsfuncHtml +
        "<script>\n//注册主题\n" +
        "let theme = " +
        JSON.stringify(themeForm.themeColor) +
        ";" +
        "\t echarts.registerTheme('customTheme', theme); </script>";

      // 判断是否是自适应
      let isSelfAdaption = themeForm.isSelfAdaption;
      //自适应类型
      let adaptionType = themeForm.adaptionType;
      //获取编辑页宽高
      let rectWidth = themeForm.panelWidth;
      let rectHeight = themeForm.panelHeight;
      //是自适应
      if (isSelfAdaption) {
        //全自适应
        if (typeof adaptionType == "undefined" || adaptionType == "0") {
          //遍历组件重新计算自适应宽高
          drawingList.forEach((item) => {
            item.width = ((item.width / rectWidth) * 100).toFixed(2) + "%";
            item.height = ((item.height / rectHeight) * 100).toFixed(2) + "%";
            item.x = ((item.x / rectWidth) * 100).toFixed(2) + "%";
            item.y = ((item.y / rectHeight) * 100).toFixed(2) + "%";
          });
        }
        //宽度自适应
        else if (adaptionType == "1") {
          //遍历组件重新计算自适应宽高
          drawingList.forEach((item) => {
            item.width = ((item.width / rectWidth) * 100).toFixed(2) + "%";
            item.height = item.height + "px";
            item.x = ((item.x / rectWidth) * 100).toFixed(2) + "%";
            item.y = item.y + "px";
          });
        }
        //高度自适应
        else if (adaptionType == "2") {
          //遍历组件重新计算自适应宽高
          drawingList.forEach((item) => {
            item.width = item.width + "px";
            item.height = ((item.height / rectHeight) * 100).toFixed(2) + "%";
            item.x = item.x + "px";
            item.y = ((item.y / rectHeight) * 100).toFixed(2) + "%";
          });
        }
      } else {
        //遍历组件重新计算自适应宽高
        drawingList.forEach((item) => {
          // item.width = (item.width/rectWidth) * document.body.offsetWidth;
          // item.height = (item.height/rectHeight) * document.body.offsetHeight;
          // item.x = (item.x/rectWidth) * document.body.offsetWidth;
          // item.y = (item.y/rectHeight) * document.body.offsetHeight;
          item.width = item.width + "px";
          item.height = item.height + "px";
          item.x = item.x + "px";
          item.y = item.y + "px";
        });
      }

      // drawingList.forEach(item => {
      //     // item.width = (item.width/rectWidth) * document.body.offsetWidth;
      //     // item.height = (item.height/rectHeight) * document.body.offsetHeight;
      //     // item.x = (item.x/rectWidth) * document.body.offsetWidth;
      //     // item.y = (item.y/rectHeight) * document.body.offsetHeight;
      //     item.width = (item.width/rectWidth  * 100).toFixed(2);
      //     item.height = (item.height/rectHeight  * 100).toFixed(2);
      //     item.x = (item.x/rectWidth  * 100).toFixed(2);
      //     item.y = (item.y/rectHeight  * 100).toFixed(2);
      //   });

      let width = themeForm.isSelfAdaption
        ? window.screen.width + "px"
        : themeForm.panelWidth + "px";
      let height = themeForm.isSelfAdaption
        ? window.screen.height + "px"
        : themeForm.panelHeight + "px";
      let background;

      // console.log(document.body.scrollWidth,document.body.scrollHeight)
      // console.log( window.screen.height, window.screen.width)

      if (themeForm.bgImage != "") {
        imgFolder.file(
          "background.png",
          getBlob(`${process.env.VUE_APP_BASE_API + themeForm.bgImage}`),
          { base64: true }
        );
        background = "url(images/background.png) no-repeat";
      } else {
        background = themeForm.bgColor;
      }

      bodyHtml =
        bodyHtml +
        '<div id="panel" style="width: ' +
        width +
        "; height: " +
        height +
        "; background: " +
        background +
        ' ">';

      //根据drawingList数据画图
      for (const ele of drawingList) {
        //let divId = ele.chartType+ele.customId;
        // 背景框下载
        if (ele.chartType == "backborder") {
          bodyHtml =
            bodyHtml +
            '<div style="left: ' +
            ele.x +
            "; top: " +
            ele.y +
            "; width: " +
            ele.width +
            "; height: " +
            ele.height +
            "; position: absolute; z-index: " +
            ele.zindex +
            ';">' +
            "<div id='div" +
            ele.customId +
            "' class='" +
            ele.chartOption.animate +
            "' style='height:100%; width: 100%;'>" +
            '<img style="height: 100%; width: 100%; opacity: ' +
            ele.chartOption.opacity +
            " \" src='images/border" +
            ele.customId +
            '.png\' alt="">' +
            "</div>" +
            "</div>";

          imgFolder.file(
            "border" + ele.customId + ".png",
            getBlob(process.env.VUE_APP_BASE_API + ele.chartOption.border),
            { base64: true }
          );
        }
        //  ------------------文本组件---------------------

        // 文本
        else if (ele.chartType == "text") {
          const style = `color:${ele.chartOption.fontColor};font-size:${ele.chartOption.fontSize}px;font-family:${ele.chartOption.fontFamily};line-height:${ele.chartOption.lineHeight}px;background-color: ${ele.chartOption.backgroundColor};height: 100%; width: 100%;font-weight:${ele.chartOption.fontWeight};letter-spacing:${ele.chartOption.letterSpacing}px;text-align:${ele.chartOption.textAlign}`;

          bodyHtml =
            bodyHtml +
            '<div style="left: ' +
            ele.x +
            "; top: " +
            ele.y +
            "; width: " +
            ele.width +
            "; height: " +
            ele.height +
            "; position: absolute; z-index: " +
            ele.zindex +
            ';">' +
            "<div id='div" +
            ele.customId +
            "' class='" +
            ele.chartOption.animate +
            "' style='height:100%; width: 100%;'>" +
            "<div style='" +
            style +
            "'>" +
            ele.chartOption.staticDataValue +
            "</div>" +
            "</div>" +
            "</div>";
        }
        // 实时时间
        else if (ele.chartType == "date") {
          const style = `color: ${ele.chartOption.fontColor};font-size:${ele.chartOption.fontSize}px;font-family:${ele.chartOption.fontFamily};height: 100%; width: 100%;background-color: ${ele.chartOption.backgroundColor};font-weight:${ele.chartOption.fontWeight};letter-spacing:${ele.chartOption.letterSpacing}px;text-align:${ele.chartOption.textAlign}`;

          bodyHtml =
            bodyHtml +
            '<div style="left: ' +
            ele.x +
            "; top: " +
            ele.y +
            "; width: " +
            ele.width +
            "; height: " +
            ele.height +
            "; position: absolute; z-index: " +
            ele.zindex +
            ';">' +
            "<div id='div" +
            ele.customId +
            "' class='" +
            ele.chartOption.animate +
            "' style='height:100%; width: 100%;'>" +
            "<div id='divChild" +
            ele.customId +
            "' style='" +
            style +
            " '></div>" +
            "</div>" +
            "</div>";

          let customFormat = ele.chartOption.customFormat.replace(/\n|\r/g, "");
          jsfuncHtml =
            jsfuncHtml +
            "<script>\n//初始化实时时间\n" +
            '\t setInterval("nowTime()",1000);\n' +
            "\t function nowTime(){ \n" +
            "\t \t	var d = new Date(); \n" +
            '\t \t	var customFormat = "' +
            customFormat +
            '"; \n';
          if (customFormat.indexOf("yyyy") != -1) {
            jsfuncHtml =
              jsfuncHtml +
              '\t \t	var dateYear = ("0000" + d.getFullYear()).slice(-4); \n' +
              '\t \t	customFormat = customFormat.replace("yyyy",dateYear); \n';
          }
          if (customFormat.indexOf("MM") != -1) {
            jsfuncHtml =
              jsfuncHtml +
              '\t \t	var dateMonth = ("00" + d.getMonth()).slice(-2); \n' +
              '\t \t	customFormat = customFormat.replace("MM",dateMonth); \n';
          }
          if (customFormat.indexOf("dd") != -1) {
            jsfuncHtml =
              jsfuncHtml +
              '\t \t	var dateDay = ("00" + d.getDate()).slice(-2); \n' +
              '\t \t	customFormat = customFormat.replace("dd",dateDay); \n';
          }
          if (customFormat.indexOf("HH") != -1) {
            jsfuncHtml =
              jsfuncHtml +
              '\t \t	var dateHour = ("00" + d.getHours()).slice(-2); \n' +
              '\t \t	customFormat = customFormat.replace("HH",dateHour); \n';
          }
          if (customFormat.indexOf("hh") != -1) {
            jsfuncHtml =
              jsfuncHtml +
              '\t \t	var datehour = ("00" + (d.getHours() >= 12 ? d.getHours() - 12 : d.getHours())).slice(-2); \n' +
              '\t \t	customFormat = customFormat.replace("hh",datehour); \n';
          }
          if (customFormat.indexOf("mm") != -1) {
            jsfuncHtml =
              jsfuncHtml +
              '\t \t	var dateMinute = ("00" + d.getMinutes()).slice(-2); \n' +
              '\t \t	customFormat = customFormat.replace("mm",dateMinute); \n';
          }
          if (customFormat.indexOf("ss") != -1) {
            jsfuncHtml =
              jsfuncHtml +
              '\t \t	var dateSecond = ("00"+d.getSeconds()).slice(-2); \n' +
              '\t \t	customFormat = customFormat.replace("ss",dateSecond); \n';
          }
          if (customFormat.indexOf("SSS") != -1) {
            jsfuncHtml =
              jsfuncHtml +
              "\t \t	var dateMill = d.getMilliseconds(); \n" +
              '\t \t	customFormat = customFormat.replace("SSS",dateMill) ;\n';
          }
          if (customFormat.indexOf("day") != -1) {
            jsfuncHtml =
              jsfuncHtml +
              "\t \t	var dateWeek = d.getDay(); \n" +
              "\t \t	switch(dateWeek){ \n" +
              '\t \t	\t	case 0: dateWeek = "星期日";break; \n' +
              '\t \t	\t	case 1: dateWeek = "星期一";break; \n' +
              '\t \t	\t	case 2: dateWeek = "星期二";break; \n' +
              '\t \t	\t	case 3: dateWeek = "星期三";break; \n' +
              '\t \t	\t	case 4: dateWeek = "星期四";break; \n' +
              '\t \t	\t	case 5: dateWeek = "星期五";break; \n' +
              '\t \t	\t	case 6: dateWeek = "星期六";break; \n' +
              "\t \t	} \n" +
              '\t \t	customFormat = customFormat.replace("day",dateWeek); \n';
          }
          jsfuncHtml =
            jsfuncHtml +
            "\t \t	document.getElementById('divChild" +
            ele.customId +
            "').innerHTML= customFormat;\n" +
            "\t } \n" +
            "</script>\n";
        }
        //跑马灯
        else if (ele.chartType == "lamp") {
          const textArr = ele.chartOption.staticDataValue.split("|");

          const style = `color:${ele.chartOption.fontColor};font-size:${ele.chartOption.fontSize}px;font-family:${ele.chartOption.fontFamily};
                        line-height:${ele.chartOption.lineHeight}px;background-color: ${ele.chartOption.backgroundColor};height: 100%; width: 100%;overflow:hidden;position: relative;
                        font-weight:${ele.chartOption.fontWeight};letter-spacing:${ele.chartOption.letterSpacing}px;text-align:${ele.chartOption.textAlign}`;

          bodyHtml =
            bodyHtml +
            '<div style="left: ' +
            ele.x +
            "; top: " +
            ele.y +
            "; width: " +
            ele.width +
            "; height: " +
            ele.height +
            "; position: absolute; z-index: " +
            ele.zindex +
            ';">' +
            "<div id='div" +
            ele.customId +
            "' class='" +
            ele.chartOption.animate +
            "' style='height:100%; width: 100%;overflow: hidden;'>" +
            "<div style='" +
            style +
            "'>";

          for (let i = 0; i < textArr.length; i++) {
            //根据字符大小及个数计算div宽度
            let width =
              textArr[i].length * parseInt(ele.chartOption.fontSize) + 5;
            bodyHtml =
              bodyHtml +
              '<div style="width:' +
              width +
              'px;position:absolute;">' +
              textArr[i] +
              "</div>";
          }

          bodyHtml = bodyHtml + "</div>" + "</div>" + "</div>";

          jsfuncHtml =
            jsfuncHtml +
            "<script>\n//初始化跑马灯\n" +
            "\t LampOption.init('div" +
            ele.customId +
            "'," +
            ele.chartOption.loopDelay +
            ");\n" +
            "</script>\n";

          jsFolder.file("lamptext.js", getBlob("/codegen/js/lamptext.js"));
          jsLinkHtml =
            jsLinkHtml +
            '<script type="text/javascript" src="js/lamptext.js"></script>';
        }
        //----------------媒体组件-------------------
        //图片
        else if (ele.chartType == "img") {
          bodyHtml =
            bodyHtml +
            '<div style="left: ' +
            ele.x +
            "; top: " +
            ele.y +
            "; width: " +
            ele.width +
            "; height: " +
            ele.height +
            "; position: absolute; z-index: " +
            ele.zindex +
            ';">' +
            "<div id='div" +
            ele.customId +
            "' class='" +
            ele.chartOption.animate +
            "' style='height:100%; width: 100%;'>" +
            '<img  style="height:100%; width: 100%;' +
            "opacity: " +
            ele.chartOption.opacity +
            "\" src='images/img" +
            ele.customId +
            '.png\' alt="">' +
            "</div>" +
            "</div>";

          imgFolder.file(
            "img" + ele.customId + ".png",
            getBlob(ele.chartOption.imgURL),
            { base64: true }
          );
        }
        //视频
        else if (ele.chartType == "video") {
          bodyHtml =
            bodyHtml +
            '<div style="left: ' +
            ele.x +
            "; top: " +
            ele.y +
            "; width: " +
            ele.width +
            "; height: " +
            ele.height +
            "; position: absolute; z-index: " +
            ele.zindex +
            ';">' +
            "<div id='div" +
            ele.customId +
            "' class='" +
            ele.chartOption.animate +
            "' style='height:100%; width: 100%;'>" +
            '<video  style="height:100%; width: 100%;" align="center" controls="true" loop="' +
            ele.chartOption.replay +
            '"> <source src="' +
            ele.chartOption.videoURL +
            '" type="video/mp4"> </video>' +
            "</div>" +
            "</div>";
        }
        //二维码
        else if (ele.chartType == "qRcode") {
          bodyHtml =
            bodyHtml +
            '<div style="left: ' +
            ele.x +
            "; top: " +
            ele.y +
            "; width: " +
            ele.width +
            "; height: " +
            ele.height +
            "; position: absolute; z-index: " +
            ele.zindex +
            ';">' +
            "<div id='div" +
            ele.customId +
            "' class='" +
            ele.chartOption.animate +
            "' style='height:100%; width: 100%;'>" +
            "</div>" +
            "</div>";

          let qrcode = {
            text: ele.chartOption.text,
            render: "canvas",
            width: ele.chartOption.size,
            height: ele.chartOption.size,
          };

          jsfuncHtml =
            jsfuncHtml +
            "<script>\n//初始化二维码\n" +
            '\t $("#div' +
            ele.customId +
            '").empty();\n' +
            '\t $("#div' +
            ele.customId +
            '").qrcode(' +
            JSON.stringify(qrcode) +
            ");\n" +
            "</script>\n";
          jsFolder.file(
            "jquery.qrcode.min.js",
            getBlob("/codegen/js/jquery.qrcode.min.js")
          );
          jsLinkHtml =
            jsLinkHtml +
            '<script type="text/javascript" src="js/jquery.qrcode.min.js"></script>';
        }
        //iframe
        else if (ele.chartType == "iframe") {
          const iframeStyle = ` background-color: ${ele.chartOption.backgroundColor}; height:100%;width:100%;position:absolute;display:block;z-index:${ele.zindex}`;
          const divStyle = `height:90%;width:90%;left:5%;top:5%;position:absolute`;

          bodyHtml =
            bodyHtml +
            '<div style="left: ' +
            ele.x +
            "; top: " +
            ele.y +
            "; width: " +
            ele.width +
            "; height: " +
            ele.height +
            "; position: absolute; z-index: " +
            ele.zindex +
            ';">' +
            "<div id='div" +
            ele.customId +
            "' class='" +
            ele.chartOption.animate +
            "' style='height:100%; width: 100%;'>" +
            '<div style="' +
            divStyle +
            '">' +
            '<iframe src="' +
            ele.chartOption.src +
            '" frameborder="0" allowtransparency="true" style="' +
            iframeStyle +
            '">您的浏览器不支持嵌入式框架，或者当前配置为不显示嵌入式框架。</iframe>' +
            "</div>" +
            "</div>" +
            "</div>";
        }
        //超链接
        else if (ele.chartType == "hyperlink") {
          const normalTextStyle = `color:${ele.chartOption.label.linkColor};font-size:${ele.chartOption.label.fontSize}px;font-family:${ele.chartOption.label.fontFamily};
                    position:absolute;cursor:pointer;user-select:none;text-decoration:${ele.chartOption.label.decoration}`;

          const imgStyle = `height:${ele.chartOption.img.width}px; width: ${ele.chartOption.img.height}px`;

          bodyHtml =
            bodyHtml +
            '<div style="left: ' +
            ele.x +
            "; top: " +
            ele.y +
            "; width: " +
            ele.width +
            "; height: " +
            ele.height +
            "; position: absolute; z-index: " +
            ele.zindex +
            ';">' +
            "<div id='div" +
            ele.customId +
            "' class='" +
            ele.chartOption.animate +
            "' style='height:100%; width: 100%;'>";
          if (ele.chartOption.type == "label") {
            bodyHtml =
              bodyHtml +
              "<a  id='hyper" +
              ele.customId +
              "' href=\"" +
              ele.chartOption.href +
              '" target="' +
              ele.chartOption.target +
              '" style="' +
              normalTextStyle +
              "\" onmouseover='visible()' onmouseout='invisible()'>" +
              ele.chartOption.label.context +
              "</a>" +
              "</div>" +
              "</div>";
          } else {
            bodyHtml =
              bodyHtml +
              '<a href="' +
              ele.chartOption.href +
              '" target="' +
              ele.chartOption.target +
              "\" ><img src='images/hyperlink" +
              ele.customId +
              '.png\'  alt="" style="' +
              imgStyle +
              '"></a>' +
              "</div>" +
              "</div>";

            imgFolder.file(
              "hyperlink" + ele.customId + ".png",
              getBlob(process.env.VUE_APP_BASE_API + ele.chartOption.img.src),
              { base64: true }
            );
          }

          jsfuncHtml =
            jsfuncHtml +
            "<script>\n//超链接鼠标移入移出事件\n" +
            '\t function visible(){ let hyperlink = document.getElementById("hyper' +
            ele.customId +
            "\"); hyperlink.style.color = '" +
            ele.chartOption.label.hoverColor +
            "'}\n" +
            '\t function invisible(){ let hyperlink = document.getElementById("hyper' +
            ele.customId +
            "\"); hyperlink.style.color = '" +
            ele.chartOption.label.linkColor +
            "'}\n" +
            "</script>\n";
        }
        //轮播图
        else if (ele.chartType == "rotation") {
          //获取图片路径
          let imageHtmlStr = "";
          for (let value of ele.chartOption.staticDataValue) {
            imageHtmlStr =
              imageHtmlStr +
              '<div class="swiper-slide"><img width="100%" height="100%" src="' +
              value.value +
              '"></div>';
          }
          let classId = "visual_swiper" + ele.customId;

          bodyHtml =
            bodyHtml +
            '<div style="left: ' +
            ele.x +
            "; top: " +
            ele.y +
            "; width: " +
            ele.width +
            "; height: " +
            ele.height +
            "; position: absolute; z-index: " +
            ele.zindex +
            ';">' +
            "<div id='div" +
            ele.customId +
            "' class='" +
            ele.chartOption.animate +
            "' style='height:100%; width: 100%;'>" +
            '<div class="visual_box">' +
            '<div class="swiper-container ' +
            classId +
            ' visual_chart">' +
            '<div class="swiper-wrapper">' +
            imageHtmlStr +
            "</div>" +
            '<div class="swiper-pagination swiper-pagination-bullets"></div>' +
            "</div>" +
            "</div>";
          "</div>" + "</div>";

          //构造轮播执行方法
          jsfuncHtml =
            jsfuncHtml +
            "<script>\n//轮播图执行方法\n" +
            "\t let rotation = $('#' +" +
            ele.customId +
            "); \n" +
            "\t rotation.empty(); \n" +
            "\t var mySwiper1 = new Swiper('." +
            classId +
            "', { \n" +
            "\t pagination: { \n" +
            "\t    el: '.swiper-pagination', \n" +
            "\t    clickable: true, \n" +
            "\t    type: '" +
            ele.chartOption.paginationType +
            "', \n" +
            "\t    hideOnClick : " +
            ele.chartOption.isPagination +
            " \n" +
            "\t }, \n" +
            "\t direction: '" +
            ele.chartOption.direction +
            "' , \n" +
            "\t loop: true, \n" +
            "\t centeredSlides: true, \n" +
            "\t autoplay: true, \n" +
            "\t speed: 1000, \n" +
            "\t autoplay: { \n" +
            "\t   delay: " +
            ele.chartOption.playSpeed +
            ", \n" +
            "\t   stopOnLastSlide: false, \n" +
            "\t   disableOnInteraction: false, \n" +
            "\t },\n" +
            "\t spaceBetween: 20, \n" +
            // autoplayDisableOnInteraction: true, //在点击之后可以继续实现轮播
            "\t effect: '" +
            ele.chartOption.isEffect +
            "', \n" +
            "\t observer: true, \n" +
            "\t observeParents: true, \n" +
            "\t }) \n" +
            "</script>\n";

          //引入js文件以及css文件
          jsFolder.file("swiper.min.js", getBlob("/codegen/js/swiper.min.js"));
          jsLinkHtml =
            jsLinkHtml +
            '<script type="text/javascript" src="js/swiper.min.js"></script>';
          cssFolder.file(
            "swiper.min.css",
            getBlob("/codegen/css/swiper.min.css")
          );
          cssLinkHtml =
            cssLinkHtml + '<link rel="stylesheet" href="css/swiper.min.css">';
          cssFolder.file("visual.css", getBlob("/codegen/css/visual.css"));
          cssLinkHtml =
            cssLinkHtml + '<link rel="stylesheet" href="css/visual.css">';
        }
        //-------------指标组件--------------
        //颜色块
        else if (ele.chartType == "colorBlock") {
          //格式化数值
          let dataValue = [];
          if (ele.chartOption.isThousand != false) {
            for (let x in ele.chartOption.staticDataValue) {
              dataValue[x] = thousandDevide(
                ele.chartOption.staticDataValue[x].value,
                ele.chartOption.isThousand
              );
            }
          } else {
            for (let x in ele.chartOption.staticDataValue) {
              dataValue[x] = ele.chartOption.staticDataValue[x].value;
            }
          }

          function thousandDevide(data, devide) {
            //格式化数字
            if (typeof data != "number") {
              return data;
            } else {
              if (!devide) {
                devide = ",";
              }
              let res = data.toString().replace(/\d+/, function (n) {
                // 先提取整数部分
                return n.replace(/(\d)(?=(\d{3})+$)/g, function ($1) {
                  return $1 + devide;
                });
              });
              return res;
            }
          }

          //构造行、文本、数值、单位样式
          const colorTextStyle = `justify-content:${ele.chartOption.text.left};font-size:${ele.chartOption.text.size}px;font-family:${ele.chartOption.text.family};
                        font-weight:${ele.chartOption.text.weight};color:${ele.chartOption.text.color};display:flex;align-items:center`;

          const colorValueStyle = `display:flex;align-items:center;justify-content:${ele.chartOption.value.left}`;

          const spanValueStyle = `font-size:${ele.chartOption.value.size}px;font-family:${ele.chartOption.value.family};font-weight:${ele.chartOption.value.weight};
                        color:${ele.chartOption.value.color}`;

          const spanSuffixStyle = `display:inline-block;margin-left:5px;text-align:left;font-size:${ele.chartOption.suffix.size}px;font-family:${ele.chartOption.suffix.family};
                        font-weight:${ele.chartOption.suffix.weight};color:${ele.chartOption.suffix.color};width:${ele.chartOption.suffix.width}%`;

          //构造行列数
          let length = ele.chartOption.colorBlock.length;

          let str = "";
          let col = 0;

          //构造颜色块
          for (let i = 0; i < ele.chartOption.staticDataValue.length; ) {
            str +=
              '<div class="color-top"></div>' +
              "<div class=\"color-row\" style='display:flex;flex-direction:row'>";
            for (
              let j = 0;
              j < ele.chartOption.numOfCol &&
              i < ele.chartOption.staticDataValue.length;
              j++
            ) {
              str +=
                '<div class="color-block" style="height:100%;display:flex;margin-left:' +
                ele.chartOption.marginLeft +
                "%" +
                ";border-radius:" +
                ele.chartOption.bradius +
                "px" +
                '">' +
                '<div class="color-text" style=\'' +
                colorTextStyle +
                "'>" +
                ele.chartOption.staticDataValue[i].name +
                "</div>" +
                '<div class="color-value" style=\'' +
                colorValueStyle +
                "'><span class=\"color-value\" style='" +
                spanValueStyle +
                "'>" +
                dataValue[i] +
                "</span>&nbsp" +
                '<span class="color-suffix" style=\'' +
                spanSuffixStyle +
                "'>&nbsp" +
                ele.chartOption.staticDataValue[i].suffix +
                "</span></div>" +
                "</div>";
              i++;
            }
            col++;
            str += "</div>";
          }

          bodyHtml =
            bodyHtml +
            '<div style="left: ' +
            ele.x +
            "; top: " +
            ele.y +
            "; width: " +
            ele.width +
            "; height: " +
            ele.height +
            "; position: absolute; z-index: " +
            ele.zindex +
            ';">' +
            "<div id='div" +
            ele.customId +
            "' class='" +
            ele.chartOption.animate +
            "' style='height:100%; width: 100%;'>" +
            str +
            "</div>" +
            "</div>";

          //计算行列宽高百分比
          let colorwidth =
            (100 - ele.chartOption.marginLeft * ele.chartOption.numOfCol) /
            ele.chartOption.numOfCol;
          let colorheight = (100 - col * ele.chartOption.marginTop) / col;

          const colorTopStyle = `({"height":"${ele.chartOption.marginTop}%"})`;
          const colorRowStyle = `({"display": "flex","height": "${colorheight}%","flex-direction": "row"})`;
          let locateColorValueStyle = ``;
          let locateColorTextStyle = ``;
          let locateColorBlockStyle = ``;

          if (ele.chartOption.textLocate == "left") {
            locateColorBlockStyle = `({"flex-direction": "row","height": "100%","width": "${colorwidth}%","margin-left":"${ele.chartOption.marginLeft}%","border-radius":"${ele.chartOption.bradius}px"})`;
            locateColorTextStyle = `({"width":'${ele.chartOption.text.width}%',"height": "100%","line-height":"100%"})`;
            locateColorValueStyle = `({"width":'${ele.chartOption.text.width}%',"height": "100%","line-height":"100%"})`;
          } else if (ele.chartOption.textLocate == "right") {
            locateColorBlockStyle = `({"flex-direction": "row-reverse","height": "100%","width":"${colorwidth}%","margin-left":"${ele.chartOption.marginLeft}%","border-radius":"${ele.chartOption.bradius}px"})`;
            locateColorTextStyle = `({"width":'${ele.chartOption.text.width}%',"height": "100%","line-height":"100%"})`;
            locateColorValueStyle = `({"width":'${ele.chartOption.text.width}%',"height": "100%","line-height":"100%"})`;
          } else if (ele.chartOption.textLocate == "top") {
            locateColorBlockStyle = `({"flex-direction": "column","height": "100%","width":"${colorwidth}%","margin-left":"${ele.chartOption.marginLeft}%","border-radius":"${ele.chartOption.bradius}px"})`;
            locateColorTextStyle = `({"width":"100%","height": "50%","line-height":"100%"})`;
            locateColorValueStyle = `({"width":"100%","height": "50%","line-height":"100%"})`;
          } else {
            locateColorBlockStyle = `({"flex-direction": "column-reverse","height": "100%","width": "${colorwidth}%","margin-left":"${ele.chartOption.marginLeft}%","border-radius":"${ele.chartOption.bradius}px"})`;
            locateColorTextStyle = `({"width":"100%","height": "50%","line-height":"100%"})`;
            locateColorValueStyle = `({"width":"100%","height": "50%","line-height":"100%"})`;
          }

          //设置颜色块行列宽高
          jsfuncHtml =
            jsfuncHtml +
            "<script>\n//设置颜色块行列宽高\n" +
            "\t let colorBlock = $('#div' +" +
            ele.customId +
            "); \n" +
            "\t colorBlock.find('div.color-row').css" +
            colorRowStyle +
            "; \n" +
            "\t colorBlock.find('div.color-top').css" +
            colorTopStyle +
            "; \n" +
            "\t colorBlock.find('div.color-block').css" +
            locateColorBlockStyle +
            "; \n" +
            "\t colorBlock.find('div.color-text').css" +
            locateColorTextStyle +
            "; \n" +
            "\t colorBlock.find('div.color-value').css" +
            locateColorValueStyle +
            "; \n" +
            "</script>\n";
        }
        // 翻牌器（静态代码下载不支持翻牌效果）
        else if (ele.chartType == "flop") {
          const style = `font-size: ${ele.chartOption.fontSize}px; font-family: ${ele.chartOption.fontFamily}; font-weight: ${ele.chartOption.fontWeight}; color: ${ele.chartOption.fontColor}; background-color: ${ele.chartOption.backgroundColor}; letter-spacing: ${ele.chartOption.letterSpacing}px; display: block; margin: 10px 0px; text-align: center;`;

          bodyHtml =
            bodyHtml +
            '<div style="left: ' +
            ele.x +
            "; top: " +
            ele.y +
            "; width: " +
            ele.width +
            "; height: " +
            ele.height +
            "; position: absolute; z-index: " +
            ele.zindex +
            ';">' +
            "<div id='div" +
            ele.customId +
            "' class='" +
            ele.chartOption.animate +
            "' style='height:100%; width: 100%;'>" +
            "<span style='" +
            style +
            "'>" +
            ele.chartOption.prefix +
            ele.chartOption.staticDataValue +
            ele.chartOption.suffix +
            "</span>" +
            "</div>" +
            "</div>";
        }
        //卡片
        else if (ele.chartType == "card") {
          let results = [];
          //列数，判断列数不能大于数据数组的总长度
          let colsNum = ele.chartOption.staticDataValue.length;
          if (
            ele.chartOption.numOfCol < ele.chartOption.staticDataValue.length
          ) {
            colsNum = ele.chartOption.numOfCol;
          }
          let col = 0; //卡片行数
          for (let i = 0; i < ele.chartOption.staticDataValue.length; ) {
            let rows = [];
            for (
              let j = 0;
              j < colsNum && i < ele.chartOption.staticDataValue.length;
              j++
            ) {
              //每行数据
              rows.push(ele.chartOption.staticDataValue[i]);
              i++;
            }
            results.push(rows);
            col++;
          }
          let rows = results;
          //每个卡片宽度占比
          let cardwidth =
            (100 - ele.chartOption.marginLeft * ele.chartOption.numOfCol) /
            ele.chartOption.numOfCol;
          //每个卡片高度占比
          let cardheight = (100 - col * ele.chartOption.marginTop) / col;

          //构造样式
          const rowStyle = `width:100%;display:flex;flex-direction:row;height:100%;margin-top:${ele.chartOption.marginTop}%`;

          const divStyle = `width: ${cardwidth}%;height: 100%;position: relative;margin-left:${ele.chartOption.marginLeft}%`;

          const headStyle = `width: 100%;height: ${ele.chartOption.head.height}%;margin-top:${ele.chartOption.marginTop}%;position: relative`;

          const headImgStyle = `width: ${ele.chartOption.head.headImg.width}%;height: ${ele.chartOption.head.headImg.height}%;margin-left:${ele.chartOption.head.headImg.marginLeft}%;
                                    position: absolute`;

          const tipImgStyle = `width:${ele.chartOption.head.tipImg.width}%;height:${ele.chartOption.head.tipImg.height}%;
                        margin-left:${ele.chartOption.head.tipImg.marginLeft}%;margin-top:${ele.chartOption.head.tipImg.marginTop}%;position: absolute`;

          const tipTextStyle = `color: ${ele.chartOption.head.tipText.fontColor};font-size: ${ele.chartOption.head.tipText.fontSize}px;
                        font-family:${ele.chartOption.head.tipText.fontFamily};font-weight: ${ele.chartOption.head.tipText.fontWeight};
                        letter-spacing: ${ele.chartOption.head.tipText.letterSpacing}px;margin-left:${ele.chartOption.head.tipText.marginLeft}%;
                        margin-top:${ele.chartOption.head.tipText.marginTop}%;position: absolute`;

          const nameStyle = `color: ${ele.chartOption.name.fontColor};font-size: ${ele.chartOption.name.fontSize}px;
                        font-family: ${ele.chartOption.name.fontFamily};font-weight: ${ele.chartOption.name.fontWeight};letter-spacing: ${ele.chartOption.name.letterSpacing}px;
                        margin-left:${ele.chartOption.name.marginLeft}%;margin-top:${ele.chartOption.name.marginTop}%;
                        margin-bottom:${ele.chartOption.name.marginBottom}%;position: relative`;

          const detailStyle = `display: flex; flex-direction:column;position:relative;color: ${ele.chartOption.detailItem.fontColor};
                        font-size: ${ele.chartOption.detailItem.fontSize}px;font-family: ${ele.chartOption.detailItem.fontFamily};
                        font-weight: ${ele.chartOption.detailItem.fontWeight};letter-spacing:${ele.chartOption.detailItem.letterSpacing}px;
                        margin-left:3%;width:94%;height: ${ele.chartOption.detailItem.height}%`;

          let str = "";

          for (let i = 0; i < rows.length; i++) {
            str +=
              "<div style='" +
              rowStyle +
              "'>" +
              "<div style='" +
              divStyle +
              "'>";
            for (let j = 0; j < rows[i].length; j++) {
              imgFolder.file(
                "card" + ele.customId + ".png",
                getBlob(ele.chartOption.head.tipImg.src),
                { base64: true }
              );

              str +=
                "<div style='" +
                headStyle +
                "'>" +
                "<img style='" +
                headImgStyle +
                "' src='" +
                rows[i][j].head.headImg +
                "'>" +
                "<img style='" +
                tipImgStyle +
                "' src='images/card" +
                ele.customId +
                ".png'>" +
                "<div style='" +
                tipTextStyle +
                "'>" +
                "<span>" +
                rows[i][j].head.tipText +
                "</span>" +
                "</div>" +
                "</div>" +
                "<div style='" +
                nameStyle +
                "'>" +
                rows[i][j].name +
                "</div>";

              //计算平均每行高度
              const itemRows = rows[i][j].infoItem.length;

              const height =
                (100 - ele.chartOption.infoItem.marginTop * itemRows) /
                itemRows;

              const itemStyle = `display:flex;justify-content:space-between;color:${ele.chartOption.infoItem.fontColor};
                                            font-size:${ele.chartOption.infoItem.fontSize}px;font-family:${ele.chartOption.infoItem.fontFamily};
                                            font-weight:${ele.chartOption.infoItem.fontWeight};letter-spacing:${ele.chartOption.infoItem.letterSpacing}px;
                                            margin-left:3%;margin-top:${ele.chartOption.infoItem.marginTop}%;width:94%;height:${height}%`;

              str +=
                "<div style='display:flex;flex-direction:column;position:relative;height:" +
                ele.chartOption.infoItem.height +
                "%'>";

              for (let k = 0; k < rows[i][j].infoItem.length; k++) {
                str +=
                  "<div style='" +
                  itemStyle +
                  "'>" +
                  "<div style='margin-left:" +
                  ele.chartOption.infoItem.textMarginLeft +
                  "%" +
                  ";margin-top:" +
                  ele.chartOption.infoItem.textMarginTop +
                  "%" +
                  "'>" +
                  rows[i][j].infoItem[k].label +
                  "</div>" +
                  "<div style='margin-left: 3%;margin-top:" +
                  ele.chartOption.infoItem.textMarginTop +
                  "%" +
                  "'>" +
                  rows[i][j].infoItem[k].value +
                  "</div>" +
                  "</div>";
              }
              str += "</div>";

              str += "<div style='" + detailStyle + "'>";

              for (let m = 0; m < rows[i][j].detailItem.length; m++) {
                //计算平均每行高度
                const itemRows = rows[i][j].detailItem.length;

                const height =
                  (100 - ele.chartOption.detailItem.marginTop * itemRows) /
                  itemRows;

                const detailInfoStyle = `text-align: center;margin-top:${ele.chartOption.detailItem.marginTop}%;height: ${height}%`;

                str +=
                  "<div style='" +
                  detailInfoStyle +
                  "'>" +
                  "<div style='margin-top:" +
                  ele.chartOption.detailItem.textMarginTop +
                  "%" +
                  "'>" +
                  rows[i][j].detailItem[m] +
                  "</div></div>";
              }
              str += "</div>";
            }
            str += "</div>";
          }
          str += "</div>";

          bodyHtml =
            bodyHtml +
            '<div style="left: ' +
            ele.x +
            "; top: " +
            ele.y +
            "; width: " +
            ele.width +
            "; height: " +
            ele.height +
            "; position: absolute; z-index: " +
            ele.zindex +
            ';">' +
            "<div id='div" +
            ele.customId +
            "' class='" +
            ele.chartOption.animate +
            "' style='height:100%; width: 100%;overflow-y: auto;'>" +
            str +
            "</div>" +
            "</div>";
        }
        //-----------表格组件----------
        //分页表格全部展示取消分页增加滚动条
        else if (ele.chartType == "taBle") {
          const theadStyle = `height:${ele.chartOption.theadheight}px;background-color:${ele.chartOption.theadbackgroundcolor};
                        font-size:${ele.chartOption.theadfontsize}px;font-family:${ele.chartOption.theadfontfamily};color:${ele.chartOption.theadfontcolor};
                        text-align:${ele.chartOption.isLeft};vertical-align:middle;`;

          bodyHtml =
            bodyHtml +
            '<div style="left: ' +
            ele.x +
            "; top: " +
            ele.y +
            "; width: " +
            ele.width +
            "; height: " +
            ele.height +
            "; position: absolute; z-index: " +
            ele.zindex +
            ';">' +
            "<div id='div" +
            ele.customId +
            "' class='" +
            ele.chartOption.animate +
            "' style='height:100%; width: 100%;overflow: auto;'>" +
            " <table style='height:100%; width: 100%;table-layout:fixed'> <thead><tr>";

          for (let item of ele.chartOption.cols) {
            bodyHtml =
              bodyHtml +
              "<th style='" +
              theadStyle +
              "'>" +
              item.title +
              "</th>";
          }

          bodyHtml = bodyHtml + "</tr></thead>";

          let tableData = [];
          //构建表格数据数组，判断是否满足动态列绑定列值，不满足设置空
          if (ele.chartOption.cols.length > 0) {
            for (let staticValue of ele.chartOption.staticDataValue) {
              let trArr = [];

              for (let col of ele.chartOption.cols) {
                let flag = false;
                for (let item in staticValue) {
                  if (item == col.field) {
                    trArr.push(staticValue[item]);
                    flag = true;
                    break;
                  }
                }
                if (flag == false) {
                  trArr.push("");
                }
              }

              tableData.push(trArr);
            }
          }

          bodyHtml = bodyHtml + "<tbody>";

          let trStyle = `width:100%;height:${ele.chartOption.tbodyheight}px;background-color:${ele.chartOption.tbodybackgroundcolor};font-size:${ele.chartOption.tbodyfontsize}px;
                        font-family:${ele.chartOption.tbodyfontfamily};color:${ele.chartOption.tbodyfontcolor};text-align:${ele.chartOption.isLeft};vertical-align:middle;`;

          //const tdStyle = `box-sizing: border-box;overflow: hidden;text-overflow: ellipsis;white-space: normal;word-break: break-all;line-height: 23px;padding-left: 10px;padding-right: 10px;text-align: center;`;
          let tdStyle = `white-space:nowrap;overflow:hidden;text-overflow:ellipsis;`;
          for (let item of tableData) {
            bodyHtml = bodyHtml + "<tr style='" + trStyle + "'>";
            for (let col of item) {
              bodyHtml =
                bodyHtml + "<td style='" + tdStyle + "'>" + col + "</td>";
            }
            bodyHtml = bodyHtml + "</tr>";
          }

          bodyHtml = bodyHtml + "</tbody></table></div>" + "</div>";
        }
        //滚动表格
        else if (ele.chartType == "tablerotation") {
          let cols = ele.chartOption.cols;

          let theadStyle = `height:${ele.chartOption.theadheight}px;background-color:${ele.chartOption.theadbackgroundcolor};
                        font-size:${ele.chartOption.theadfontsize}px;font-family:${ele.chartOption.theadfontfamily};color:${ele.chartOption.theadfontcolor};
                        text-align:${ele.chartOption.isLeft};vertical-align:middle;`;

          bodyHtml =
            bodyHtml +
            '<div style="left: ' +
            ele.x +
            "; top: " +
            ele.y +
            "; width: " +
            ele.width +
            "; height: " +
            ele.height +
            "; position: absolute; z-index: " +
            ele.zindex +
            ';">' +
            "<div id='div" +
            ele.customId +
            "' class='" +
            ele.chartOption.animate +
            "' style='height:100%; width: 100%;'>";

          let theadWith = "";
          if (ele.chartOption.isthead == true) {
            bodyHtml =
              bodyHtml +
              " <table style='height:100%; width: 100%;table-layout:fixed'> <thead><tr>";

            for (let index in cols) {
              theadWith = "width:" + cols[index].width + "%";
              bodyHtml =
                bodyHtml +
                "<th style='" +
                theadStyle +
                theadWith +
                "'>" +
                cols[index].title +
                "</th>";
              theadWith = "";
            }

            bodyHtml = bodyHtml + "</tr></thead></table>";
          }

          let tableData = [];
          //构建表格数据数组，判断是否满足动态列绑定列值，不满足设置空
          if (ele.chartOption.cols.length > 0) {
            for (let staticValue of ele.chartOption.staticDataValue) {
              let trArr = [];

              for (let col of ele.chartOption.cols) {
                let flag = false;
                for (let item in staticValue) {
                  if (item == col.field) {
                    trArr.push(staticValue[item]);
                    flag = true;
                    break;
                  }
                }
                if (flag == false) {
                  trArr.push("");
                }
              }

              tableData.push(trArr);
            }
          }

          bodyHtml =
            bodyHtml +
            "<div id='tbody" +
            ele.customId +
            "' style='width:100%'><table style='width:100%;table-layout:fixed'><tbody style='width:100%'>";

          let trStyle = `width:100%;height:${ele.chartOption.tbodyheight}px;display:table;table-layout:fixed;font-size:${ele.chartOption.tbodyfontsize}px;
                        font-family:${ele.chartOption.tbodyfontfamily};color:${ele.chartOption.tbodyfontcolor};text-align:${ele.chartOption.isLeft};vertical-align:middle;`;

          let widthStyle = "";

          let tdStyle = `white-space:nowrap;overflow:hidden;text-overflow:ellipsis;`;

          let trColor = "";
          for (let index in tableData) {
            //奇偶行颜色
            if (parseInt(index) % 2 == 0) {
              trColor = "background-color:" + ele.chartOption.oddcolor + ";";
            } else {
              trColor = "background-color:" + ele.chartOption.evencolor + ";";
            }
            //构造表格行列数据
            bodyHtml = bodyHtml + "<tr style='" + trStyle + trColor + "'>";
            for (let colindex in tableData[index]) {
              if (cols.length > 0) {
                widthStyle = "width:" + cols[colindex].width + "%";
              }

              bodyHtml =
                bodyHtml +
                "<td style='" +
                tdStyle +
                widthStyle +
                "'>" +
                tableData[index][colindex] +
                "</td>";
              widthStyle = "";
            }
            bodyHtml = bodyHtml + "</tr>";
            trColor = "";
          }

          bodyHtml = bodyHtml + "</tbody></table></div></div>" + "</div>";

          jsfuncHtml =
            jsfuncHtml +
            "<script>\n//初始化滚动表格\n" +
            '\t $("#tbody' +
            ele.customId +
            '").liMarquee({\n' +
            "\t \t	direction: 'up', \n" +
            "\t \t	circular: true, \n" +
            "\t \t	drag: false, \n" +
            "\t \t	scrollamount: " +
            ele.chartOption.velocity * 50 +
            ", \n" +
            "\t \t	runshort: " +
            ele.chartOption.isrunshort +
            ", \n" +
            "\t }); \n" +
            "\t var rectHeight = document.body.offsetHeight;\n";

          if (isSelfAdaption) {
            jsfuncHtml =
              jsfuncHtml +
              "\t let tbodyHeight" +
              ele.customId +
              " = ((rectHeight * " +
              ele.height.replace("%", "") / 100 +
              ") - " +
              ele.chartOption.theadheight +
              ")+ 'px';\n" +
              '\t $("#tbody' +
              ele.customId +
              "\").css({'height':tbodyHeight" +
              ele.customId +
              "})\n";
          } else {
            jsfuncHtml =
              jsfuncHtml +
              "\t let tbodyHeight" +
              ele.customId +
              " = (" +
              ele.height.replace("px", "") +
              "- " +
              ele.chartOption.theadheight +
              ")+ 'px';\n" +
              '\t $("#tbody' +
              ele.customId +
              "\").css({'height':tbodyHeight" +
              ele.customId +
              "})\n";
          }

          if (ele.chartOption.isoverflowtip == true) {
            jsfuncHtml =
              jsfuncHtml +
              '\t var $td = $("#div' +
              ele.customId +
              '").find("table tbody td");\n' +
              "\t $td.hover( function (e) {\n" +
              "\t \t	if($(this)[0].scrollWidth>$(this)[0].offsetWidth){ \n" +
              '\t \t	\t	var tooltip = "<div id=\'tplink\'>" + $(this).html() + "</div>" \n' +
              '\t \t	\t	$("body").append(tooltip); \n' +
              '\t \t	\t	$("#tplink").css({ \n' +
              '\t \t	\t	\t	"z-index": "999", \n' +
              '\t \t	\t	\t	"position": "absolute", \n' +
              '\t \t	\t	\t	"top": e.pageY + "px", \n' +
              '\t \t	\t	\t	"left": e.pageX + "px", \n' +
              '\t \t	\t	\t	"color": "' +
              ele.chartOption.Ocolor +
              '", \n' +
              '\t \t	\t	\t	"font-size": ' +
              ele.chartOption.Ofontsize +
              '+"px", \n' +
              '\t \t	\t	\t	"background-color": "' +
              ele.chartOption.Obgcolor +
              '", \n' +
              "\t \t	\t	}).show();  \n" +
              "\t \t	}  \n" +
              "\t }, function () { \n" +
              '\t \t	$("#tplink").remove();  \n' +
              "\t }); \n";
          }

          jsfuncHtml = jsfuncHtml + "</script>\n";
          // 滚动表格插件
          cssFolder.file(
            "tablerotation.css",
            getBlob("/codegen/css/tablerotation.css")
          );
          cssLinkHtml =
            cssLinkHtml +
            '<link rel="stylesheet" href="css/tablerotation.css">';

          jsFolder.file(
            "jquery.liMarquee.js",
            getBlob("/codegen/js/jquery.liMarquee.js")
          );
          jsLinkHtml =
            jsLinkHtml +
            '<script type="text/javascript" src="js/jquery.liMarquee.js"></script>';
        }
        //排行表格
        else if (ele.chartType == "tabletop") {
          let cols = ele.chartOption.cols;

          let theadStyle = `height:${ele.chartOption.theadheight}px;background-color:${ele.chartOption.theadbackgroundcolor};
                        font-size:${ele.chartOption.theadfontsize}px;font-family:${ele.chartOption.theadfontfamily};color:${ele.chartOption.theadfontcolor};
                        text-align:${ele.chartOption.isLeft};vertical-align:middle;`;

          bodyHtml =
            bodyHtml +
            '<div style="left: ' +
            ele.x +
            "; top: " +
            ele.y +
            "; width: " +
            ele.width +
            "; height: " +
            ele.height +
            "; position: absolute; z-index: " +
            ele.zindex +
            ';">' +
            "<div id='div" +
            ele.customId +
            "' class='" +
            ele.chartOption.animate +
            "' style='height:100%; width: 100%;overflow: auto;'>" +
            " <table style='height:100%; width: 100%;table-layout:fixed'> ";
          if (ele.chartOption.isthead == true) {
            bodyHtml =
              bodyHtml + "<thead><tr><th style='" + theadStyle + "'>排名</th>";
            let theadWith = "";

            for (let index in cols) {
              theadWith = "width:" + cols[index].width + "%";
              bodyHtml =
                bodyHtml +
                "<th style='" +
                theadStyle +
                theadWith +
                "'>" +
                cols[index].title +
                "</th>";
              theadWith = "";
            }

            bodyHtml = bodyHtml + "</tr></thead>";
          }

          let tableData = [];
          //构建表格数据数组，判断是否满足动态列绑定列值，不满足设置空
          if (ele.chartOption.cols.length > 0) {
            for (let staticValue of ele.chartOption.staticDataValue) {
              let trArr = [];

              for (let col of ele.chartOption.cols) {
                let flag = false;
                for (let item in staticValue) {
                  if (item == col.field) {
                    trArr.push(staticValue[item]);
                    flag = true;
                    break;
                  }
                }
                if (flag == false) {
                  trArr.push("");
                }
              }

              tableData.push(trArr);
            }
          }

          bodyHtml = bodyHtml + "<tbody style='width:100%'>";

          //const tdStyle = `box-sizing: border-box;overflow: hidden;text-overflow: ellipsis;white-space: normal;word-break: break-all;line-height: 23px;padding-left: 10px;padding-right: 10px;text-align: center;`;

          let trStyle = `width:100%;height:${ele.chartOption.tbodyheight}px;table-layout:fixed;font-size:${ele.chartOption.tbodyfontsize}px;
                        font-family:${ele.chartOption.tbodyfontfamily};color:${ele.chartOption.tbodyfontcolor};text-align:${ele.chartOption.isLeft};vertical-align:middle;`;

          let widthStyle = "";

          let tdStyle = `white-space:nowrap;overflow:hidden;text-overflow:ellipsis;`;
          let topcolor = ["#ed405d", "#f78c44", "#49bcf7", "#878787"];
          let size = parseInt(ele.chartOption.tbodyfontsize) + 6;
          let spanStyle = `width:${size}px;height:${size}px;border-radius:3px;color:#fff;line-height:${size}px;text-align:center;display:inline-block;`;

          let trColor = "";
          let spanBack = "";
          for (let index in tableData) {
            //奇偶行颜色
            if (parseInt(index) % 2 == 0) {
              trColor = "background-color:" + ele.chartOption.oddcolor + ";";
            } else {
              trColor = "background-color:" + ele.chartOption.evencolor + ";";
            }
            //构造表格行列数据
            bodyHtml = bodyHtml + "<tr style='" + trStyle + trColor + "'>";
            if (parseInt(index) == 0) {
              spanBack = "background-color:" + topcolor[0];
            } else if (parseInt(index) == 1) {
              spanBack = "background-color:" + topcolor[1];
            } else if (parseInt(index) == 2) {
              spanBack = "background-color:" + topcolor[2];
            } else {
              spanBack = "background-color:" + topcolor[3];
            }

            bodyHtml =
              bodyHtml +
              "<td><span style='" +
              spanStyle +
              spanBack +
              "'>" +
              (parseInt(index) + 1) +
              "</span></td>";

            for (let colindex in tableData[index]) {
              if (cols.length > 0) {
                widthStyle = "width:" + cols[colindex].width + "%";
              }

              bodyHtml =
                bodyHtml +
                "<td style='" +
                tdStyle +
                widthStyle +
                "'>" +
                tableData[index][colindex] +
                "</td>";
              widthStyle = "";
              spanBack = "";
            }
            bodyHtml = bodyHtml + "</tr>";
            trColor = "";
          }

          bodyHtml = bodyHtml + "</tbody></table></div>" + "</div>";

          jsfuncHtml =
            jsfuncHtml +
            "<script>\n//初始化排行表格\n" +
            "\t var rectHeight = document.body.offsetHeight;\n";

          if (isSelfAdaption) {
            jsfuncHtml =
              jsfuncHtml +
              "\t let tbodyHeight" +
              ele.customId +
              " = ((rectHeight * " +
              ele.height.replace("%", "") / 100 +
              ") - " +
              ele.chartOption.theadheight +
              ")+ 'px';\n";
          } else {
            jsfuncHtml =
              jsfuncHtml +
              "\t let tbodyHeight" +
              ele.customId +
              " = (" +
              ele.height.replace("px", "") +
              "- " +
              ele.chartOption.theadheight +
              ")+ 'px';\n";
          }

          jsfuncHtml =
            jsfuncHtml +
            '\t $("#div' +
            ele.customId +
            '").find("tbody").css({\'height\':tbodyHeight' +
            ele.customId +
            "})\n" +
            '\t var $td = $("#div' +
            ele.customId +
            '").find("table tbody td");\n' +
            "\t $td.hover( function (e) {\n" +
            "\t \t	if($(this)[0].scrollWidth>$(this)[0].offsetWidth){ \n" +
            '\t \t	\t	var tooltip = "<div id=\'tplink\'>" + $(this).html() + "</div>" \n' +
            '\t \t	\t	$("body").append(tooltip); \n' +
            '\t \t	\t	$("#tplink").css({ \n' +
            '\t \t	\t	\t	"z-index": "999", \n' +
            '\t \t	\t	\t	"position": "absolute", \n' +
            '\t \t	\t	\t	"top": e.pageY + "px", \n' +
            '\t \t	\t	\t	"left": e.pageX + "px", \n' +
            '\t \t	\t	\t	"color": "' +
            ele.chartOption.Ocolor +
            '", \n' +
            '\t \t	\t	\t	"font-size": ' +
            ele.chartOption.Ofontsize +
            '+"px", \n' +
            '\t \t	\t	\t	"background-color": "' +
            ele.chartOption.Obgcolor +
            '", \n' +
            "\t \t	\t	}).show();  \n" +
            "\t \t	}  \n" +
            "\t }, function () { \n" +
            '\t \t	$("#tplink").remove();  \n' +
            "\t }); \n" +
            "</script>\n";
        }
        //---------条件组件----------
        //input输入框
        else if (ele.chartType == "input") {
          //label样式
          const normalStyle = `font-size:${ele.chartOption.label.fontSize}px;font-family:${ele.chartOption.label.fontFamily};
                        color:${ele.chartOption.label.fontColor};line-height: 40px;padding: 0 12px 0 0;box-sizing: border-box;`;
          //input样式
          const inputStyle = `color:#fff; border-radius:5px; border-color:rgba(255,255,255,0.8);background-color:rgba(0,0,0,0);
                        font-size:${ele.chartOption.label.fontSize}px;font-family:${ele.chartOption.label.fontFamily};width:${ele.chartOption.width}px`;

          bodyHtml =
            bodyHtml +
            '<div style="left: ' +
            ele.x +
            "; top: " +
            ele.y +
            "; width: " +
            ele.width +
            "; height: " +
            ele.height +
            "; position: absolute; z-index: " +
            ele.zindex +
            ';">' +
            "<div id='div" +
            ele.customId +
            "' class='" +
            ele.chartOption.animate +
            "' style='height:100%; width: 100%;display: flex;float: left;'>" +
            "<label style='" +
            normalStyle +
            "'>" +
            ele.chartOption.label.context +
            "</label>" +
            "<div style='line-height: 40px;'>" +
            '<input placeholder="请输入" id=\'input' +
            ele.customId +
            "' name='" +
            ele.chartOption.input.name +
            "' type='" +
            ele.chartOption.input.type +
            "'" +
            " maxlength='" +
            ele.chartOption.input.maxlength +
            "' style='" +
            inputStyle +
            "'></input>" +
            "</div>" +
            "</div>" +
            "</div>";
        }
        //下拉框
        else if (ele.chartType == "select") {
          //label样式
          const normalTextStyle = `font-size:${ele.chartOption.fontSize}px;font-family:${ele.chartOption.fontFamily};
                        color:${ele.chartOption.fontColor};line-height: 40px;padding: 0 12px 0 0;box-sizing: border-box;`;

          //select样式
          const selectStyle = `border-radius:5px; border-color:rgba(255,255,255,0.8);background-color:rgba(0,0,0,0);color:#606266;
                        font-size:${ele.chartOption.fontSize}px;font-family:${ele.chartOption.fontFamily};width:222px;height:36px;line-height: 36px;`;

          bodyHtml =
            bodyHtml +
            '<div style="left: ' +
            ele.x +
            "; top: " +
            ele.y +
            "; width: " +
            ele.width +
            "; height: " +
            ele.height +
            "; position: absolute; z-index: " +
            ele.zindex +
            ';">' +
            "<div id='div" +
            ele.customId +
            "' class='" +
            ele.chartOption.animate +
            "' style='height:100%; width: 100%;display: flex;float: left;'>" +
            "<label style='" +
            normalTextStyle +
            "'>" +
            ele.chartOption.context +
            "</label>" +
            "<div style='line-height: 40px;'>" +
            '<select placeholder="请选择" name=\'' +
            ele.chartOption.name +
            "' id='select" +
            ele.customId +
            "' style='" +
            selectStyle +
            "'>";
          for (let item of ele.chartOption.staticDataValue) {
            bodyHtml =
              bodyHtml +
              "<option value='" +
              item.value +
              "'>" +
              item.tab +
              "</option>";
          }

          bodyHtml = bodyHtml + "</select></div>" + "</div>" + "</div>";
        }
        //时间范围
        else if (ele.chartType == "timeframe") {
          jsfuncHtml =
            jsfuncHtml + "<script>\n//时间范围组件不支持下载\n" + "</script>\n";
        }
        //级联选择
        else if (ele.chartType == "cascade") {
          jsfuncHtml =
            jsfuncHtml + "<script>\n//级联选择组件不支持下载\n" + "</script>\n";
        }
        //选项卡
        else if (ele.chartType == "tab") {
          bodyHtml =
            bodyHtml +
            '<div style="left: ' +
            ele.x +
            "; top: " +
            ele.y +
            "; width: " +
            ele.width +
            "; height: " +
            ele.height +
            "; position: absolute; z-index: " +
            ele.zindex +
            ';">' +
            "<div id='div" +
            ele.customId +
            "' class='" +
            ele.chartOption.animate +
            "' style='height:100%; width: 100%;'>" +
            "<ul style='height:100%; width: 100%;position:relative;list-style: none;margin:0; padding:0;'>";

          const num = ele.chartOption.staticDataValue.length;

          let liStyle = ``;
          if (ele.chartOption.direction == "vertical") {
            liStyle = `height:${
              100 / num - 1
            }%;list-style: none; width: 100%; padding-left: 1px;float: left; cursor: default;`;
          } else {
            liStyle = `width:${
              100 / num - 1
            }%;list-style: none; height: 100%; padding-left: 1px;float: left; cursor: default;`;
          }

          const tabBorderStyle = `border-width:${ele.chartOption.borderWidth}px;border-color:${ele.chartOption.borderColor};
                        background-color:${ele.chartOption.backgroundColor};border-style: solid;height: 100%;position: relative;`;

          const tabTextStyle = `position: absolute;top : 38%;width: 100%;color:${ele.chartOption.fontColor};font-size:${ele.chartOption.fontSize}px;
                        font-family:${ele.chartOption.fontFamily};font-weight:${ele.chartOption.fontWeight};letter-spacing:${ele.chartOption.selectedLetterSpacing};
                        text-align:${ele.chartOption.textAlign}`;

          for (let index in ele.chartOption.staticDataValue) {
            bodyHtml =
              bodyHtml +
              "<li bindid='" +
              ele.chartOption.staticDataValue[index].bindid +
              "' style='" +
              liStyle +
              "'>" +
              "<div class='tabBorder' style='" +
              tabBorderStyle +
              "'>" +
              "<div class='tabText' style='" +
              tabTextStyle +
              "'>" +
              ele.chartOption.staticDataValue[index].content +
              "</div>" +
              "</div>" +
              "</li>";
          }

          bodyHtml = bodyHtml + "</ul></div>" + "</div>";

          jsfuncHtml =
            jsfuncHtml +
            "<script>\n//初始化选项卡\n" +
            "\t $( $('#div" +
            ele.customId +
            '\').find("li")).click(function (e) {\n' +
            "\t \t	var bindingObjs =" +
            JSON.stringify(ele.chartOption.bindingObjs) +
            ";\n" +
            "\t \t	$.each(bindingObjs, function (index, value) {\n" +
            '\t \t	\t	$("#div"+value.chartid).css("display","block");\n' +
            '\t \t	\t	var bindid = $(e.currentTarget).attr("bindid");\n' +
            '\t \t	\t	$("#div"+value.chartid).css("display","none");\n' +
            "\t \t	\t	if(bindid == value.bindid){ \n" +
            '\t \t	\t	\t	$("#div"+value.chartid).css("display","block"); \n' +
            "\t \t	\t	} \n" +
            "\t \t	}); \n" +
            "\t \t	$($('#div" +
            ele.customId +
            '\').find(".tabText")).css({ \n' +
            '\t \t	\t	"color":"' +
            ele.chartOption.fontColor +
            '", \n' +
            '\t \t	\t	"font-size":"' +
            ele.chartOption.fontSize +
            'px", \n' +
            '\t \t	\t	"font-weight":"' +
            ele.chartOption.fontWeight +
            '", \n' +
            '\t \t	\t	"font-family":"' +
            ele.chartOption.fontFamily +
            '", \n' +
            '\t \t	\t	"letter-spacing":"' +
            ele.chartOption.letterSpacing +
            'px", \n' +
            '\t \t	\t	"text-align":"' +
            ele.chartOption.textAlign +
            '", \n' +
            "\t \t	}); \n" +
            "\t \t	$($('#div" +
            ele.customId +
            '\').find(".tabBorder")).css({ \n' +
            '\t \t	\t	"border-width":"' +
            ele.chartOption.borderWidth +
            'px", \n' +
            '\t \t	\t	"border-color":"' +
            ele.chartOption.borderColor +
            '", \n' +
            '\t \t	\t	"background-color":"' +
            ele.chartOption.backgroundColor +
            '", \n' +
            "\t \t	}); \n" +
            '\t \t	$($(e.currentTarget).find(".tabText")).css({ \n' +
            '\t \t	\t	"color":"' +
            ele.chartOption.selectedFontColor +
            '", \n' +
            '\t \t	\t	"font-size":"' +
            ele.chartOption.selectedFontSize +
            'px", \n' +
            '\t \t	\t	"font-weight":"' +
            ele.chartOption.selectedFontWeight +
            '", \n' +
            '\t \t	\t	"font-family":"' +
            ele.chartOption.selectedFontFamily +
            '", \n' +
            '\t \t	\t	"letter-spacing":"' +
            ele.chartOption.selectedLetterSpacing +
            'px", \n' +
            "\t \t	}); \n" +
            '\t \t	$($(e.currentTarget).find(".tabBorder")).css({ \n' +
            '\t \t	\t	"border-width":"' +
            ele.chartOption.selectedBorderWidth +
            'px", \n' +
            '\t \t	\t	"border-color":"' +
            ele.chartOption.selectedBorderColor +
            '", \n' +
            '\t \t	\t	"background-color":"' +
            ele.chartOption.selectedBackgroundColor +
            '", \n' +
            "\t \t	}); \n" +
            "\t }); \n" +
            "\t $(document).ready(function(){ \n" +
            "\t \t	$( $('#div" +
            ele.customId +
            '\').find("li")[0]).trigger("click"); \n' +
            "\t }); \n" +
            "</script>\n";
        }
        //文本复选框
        else if (ele.chartType == "textCheckBox") {
          //文本复选框 默认样式
          const normalStyle = `padding:${ele.chartOption.paddingTopBottom}px ${ele.chartOption.paddingLeftRight}px;
                        margin:${ele.chartOption.marginTopBottom}px ${ele.chartOption.marginLeftRight}px;
                        font-size:${ele.chartOption.fontSize}px; font-weight:${ele.chartOption.fontWeight};
                        font-family:${ele.chartOption.fontFamily}; letter-spacing:${ele.chartOption.letterSpacing}px;
                        color:${ele.chartOption.fontColor}; background-color:${ele.chartOption.backgroundColor}; 
                        float:left; box-sizing:border-box; cursor:pointer`;
          //文本复选框 选中样式
          const selectedStyle = `padding:${ele.chartOption.paddingTopBottom}px ${ele.chartOption.paddingLeftRight}px;
                        margin:${ele.chartOption.marginTopBottom}px ${ele.chartOption.marginLeftRight}px;
                        font-size:${ele.chartOption.selectedFontSize}px; font-family:${ele.chartOption.selectedFontFamily};
                        font-weight:${ele.chartOption.selectedFontWeight}; letter-spacing:${ele.chartOption.selectedLetterSpacing}px;
                        color:${ele.chartOption.selectedFontColor}; background-color:${ele.chartOption.selectedBackgroundColor}; 
                        float:left; box-sizing:border-box;`;

          bodyHtml =
            bodyHtml +
            '<div style="left: ' +
            ele.x +
            "; top: " +
            ele.y +
            "; width: " +
            ele.width +
            "; height: " +
            ele.height +
            "; position: absolute; z-index: " +
            ele.zindex +
            ';">' +
            "<div id='div" +
            ele.customId +
            "' class='" +
            ele.chartOption.animate +
            "' style='height:100%; width: 100%;'>";
          var boxes = "";
          for (let item of ele.chartOption.staticDataValue) {
            boxes =
              boxes +
              '<span class="chooseCard" style=\'' +
              normalStyle +
              "'>" +
              item.value +
              "</span>";
          }
          bodyHtml = bodyHtml + boxes + "</div> </div>";

          jsfuncHtml =
            jsfuncHtml +
            "<script>\n//初始化文本复选框\n" +
            "$('span').click(function () {\r\n" +
            "   if ($(this).hasClass('onSelected')) {\r\n" +
            "       $(this).removeClass('onSelected');\r\n" +
            "       /* 默认样式 */\r\n" +
            "       $(this).css({\r\n" +
            '           "float": "left",\r\n' +
            "           /* 字体 */\r\n" +
            '           "color":"' +
            ele.chartOption.fontColor +
            '",\r\n' +
            '           "font-size":"' +
            ele.chartOption.fontSize +
            'px",\r\n' +
            '           "font-weight":"' +
            ele.chartOption.fontWeight +
            '",\r\n' +
            '           "font-family":"' +
            ele.chartOption.fontFamily +
            '",\r\n' +
            '           "letter-spacing":"' +
            ele.chartOption.letterSpacing +
            'px",\r\n' +
            '           "background-color":"' +
            ele.chartOption.backgroundColor +
            '",\r\n' +
            "           /* 内边距 */\r\n" +
            '           "padding-left":"' +
            ele.chartOption.paddingLeftRight +
            'px",\r\n' +
            '           "padding-right":"' +
            ele.chartOption.paddingLeftRight +
            'px",\r\n' +
            '           "padding-top":"' +
            ele.chartOption.paddingTopBottom +
            'px",\r\n' +
            '           "padding-bottom":"' +
            ele.chartOption.paddingTopBottom +
            'px",\r\n' +
            "           /* 外边距 */\r\n" +
            '           "margin-left":"' +
            ele.chartOption.marginLeftRight +
            'px",\r\n' +
            '           "margin-right":"' +
            ele.chartOption.marginLeftRight +
            'px",\r\n' +
            '           "margin-top":"' +
            ele.chartOption.marginTopBottom +
            'px",\r\n' +
            '           "margin-bottom":"' +
            ele.chartOption.marginTopBottom +
            'px",\r\n' +
            "           /* 鼠标触发样式 */\r\n" +
            '           "cursor": "pointer",\r\n' +
            "           });\r\n" +
            "   } else { // 被选中状态\r\n" +
            "       $(this).addClass('onSelected');\r\n" +
            "       /* 选中样式 */\r\n" +
            "       $(this).css({\r\n" +
            '           "float": "left",\r\n' +
            "           /* 字体 */\r\n" +
            '           "color":"' +
            ele.chartOption.selectedFontColor +
            '",\r\n' +
            '           "font-size":"' +
            ele.chartOption.selectedFontSize +
            'px",\r\n' +
            '           "font-weight":"' +
            ele.chartOption.selectedFontWeight +
            '",\r\n' +
            '           "font-family":"' +
            ele.chartOption.selectedFontFamily +
            '",\r\n' +
            '           "letter-spacing":"' +
            ele.chartOption.selectedLetterSpacing +
            'px",\r\n' +
            '           "background-color":"' +
            ele.chartOption.selectedBackgroundColor +
            '",\r\n' +
            "           /* 鼠标触发样式 */\r\n" +
            '           "cursor": "pointer",\r\n' +
            "       });\r\n" +
            "   }" +
            "})" +
            "</script>\n";
        } else if (ele.chartType == "pictorialBar") {
          bodyHtml =
            bodyHtml +
            '<div style="left: ' +
            ele.x +
            "; top: " +
            ele.y +
            "; width: " +
            ele.width +
            "; height: " +
            ele.height +
            "; position: absolute; z-index: " +
            ele.zindex +
            ';">' +
            "<div id='div" +
            ele.customId +
            "' class='" +
            ele.chartOption.animate +
            "' style='height:100%; width: 100%;'>" +
            "</div>" +
            "</div>";
          for (
            let index = 0;
            index < ele.chartOption.imageList.length;
            index++
          ) {
            imgFolder.file(
              "pictorialBar" + ele.customId + "-" + index + ".png",
              getBlob(
                process.env.VUE_APP_BASE_API +
                  ele.chartOption.imageList[index].imagePath
              ),
              { base64: true }
            );
          }
        } else {
          bodyHtml =
            bodyHtml +
            '<div style="left: ' +
            ele.x +
            "; top: " +
            ele.y +
            "; width: " +
            ele.width +
            "; height: " +
            ele.height +
            "; position: absolute; z-index: " +
            ele.zindex +
            ';">' +
            "<div id='div" +
            ele.customId +
            "' class='" +
            ele.chartOption.animate +
            "' style='height:100%; width: 100%;'>" +
            "</div>" +
            "</div>";
        }
      }

      for (let key in optionMap) {
        if (Object.hasOwnProperty.call(optionMap, key)) {
          const element = optionMap[key];
          //字符云
          if (
            element.bindingType != null &&
            element.bindingType == "wordcloud"
          ) {
            jsfuncHtml =
              jsfuncHtml +
              "<script>\n//初始化字符云\n" +
              "\t var myChart_" +
              key +
              " = echarts.init(document.getElementById('div" +
              key +
              "'),'customTheme');" +
              "\n" +
              "\t var option_" +
              key +
              " = " +
              JSON.stringify(optionMap[key]) +
              ";\n" +
              "\t option_" +
              key +
              ".series[0].textStyle.normal.color=function () { \n" +
              "\t	\t	return 'rgb(' + [ \n" +
              "\t	\t	Math.round(Math.random() * 255), \n" +
              "\t	\t	Math.round(Math.random() * 255), \n" +
              "\t	\t	Math.round(Math.random() * 255) \n" +
              "\t	\t	].join(',') + ')';\n" +
              "\t	} \n" +
              "\t myChart_" +
              key +
              ".setOption(option_" +
              key +
              ", true);" +
              "\n" +
              "</script>\n";

            // echarts-wordcloud.min.js依赖
            jsFolder.file(
              "echarts-wordcloud.min.js",
              getBlob("/codegen/js/echarts-wordcloud.min.js")
            );
            jsLinkHtml =
              jsLinkHtml +
              '<script type="text/javascript" src="js/echarts-wordcloud.min.js"></script>';
          }
          //伪立体柱状图
          else if (
            element.bindingType != null &&
            element.bindingType == "threeDBar"
          ) {
            jsfuncHtml =
              jsfuncHtml +
              "<script>\n//初始化伪立体柱状图\n" +
              "\t var myChart_" +
              key +
              " = echarts.init(document.getElementById('div" +
              key +
              "'),'customTheme');" +
              "\n" +
              "\t var setting = " +
              JSON.stringify(optionMap[key]) +
              ";\n" +
              "\t initRender(setting)" +
              ";\n" +
              "\t myChart_" +
              key +
              " .setOption(setting.option, true);" +
              "\n" +
              "\t myChart_" +
              key +
              " .on('legendselectchanged', function(obj) {" +
              "\n" +
              "\t var newSetting = changeSelected(setting, obj.selected);" +
              "\n" +
              "\t initRender(newSetting);" +
              "\n" +
              "\t myChart_" +
              key +
              " .setOption(newSetting.option, true);" +
              "\n" +
              "\t })" +
              "\n" +
              "</script>\n";

            // echarts-threeDBar.js依赖
            jsFolder.file(
              "echarts-threeDBar.js",
              getBlob("/codegen/js/echarts-threeDBar.js")
            );
            jsLinkHtml =
              jsLinkHtml +
              '<script type="text/javascript" src="js/echarts-threeDBar.js"></script>';
          }
          //键盘仪表盘
          else if (
            element.bindingType != null &&
            element.bindingType == "keygauge"
          ) {
            jsfuncHtml =
              jsfuncHtml +
              "<script>\n//初始化键盘仪表盘\n" +
              '\t var width = $("#div' +
              key +
              '").css("width").replace("px","");' +
              '\t var height = $("#div' +
              key +
              '").css("height").replace("px","");' +
              '\t $("#keyCanvas").remove();' +
              "\t var canvas = \"<canvas id='keyCanvas' width='\"+width+\"' height='\"+height+\"' style='display:none'>\";" +
              '\t $("#div' +
              key +
              '").append(canvas);' +
              '\t var c=document.getElementById("keyCanvas");' +
              '\t var ctx=c.getContext("2d");' +
              "\t var grd=ctx.createLinearGradient(0,0,width,0);" +
              "\t var canvasColor =" +
              JSON.stringify(optionMap[key].color) +
              ";" +
              "\t for(var i=0;i<" +
              optionMap[key].color.length +
              ";i++){" +
              "\t \t grd.addColorStop(canvasColor[i][0],canvasColor[i][1]);" +
              "\t }" +
              "\t ctx.fillStyle=grd;" +
              "\t ctx.fillRect(0,0,width,height);" +
              "\t var myChart_" +
              key +
              " = echarts.init(document.getElementById('div" +
              key +
              "'), 'customTheme');" +
              "\n" +
              "\t var option_" +
              key +
              " = " +
              JSON.stringify(optionMap[key]) +
              ";\n" +
              "\t option_" +
              key +
              ".series[0].splitLine.lineStyle.color.image = c;\n" +
              "\t myChart_" +
              key +
              ".setOption(option_" +
              key +
              ", true);" +
              "\n" +
              "</script>\n";
          }
          //地图
          else if (
            element.bindingType != null &&
            element.bindingType == "map"
          ) {
            let mapName = element.geo.map;
            if (mapName != "china") {
              jsFolder.file(
                mapName + ".js",
                getBlob("/codegen/js/province/" + mapName + ".js")
              );
              jsLinkHtml =
                jsLinkHtml +
                '<script type="text/javascript" src="js/' +
                mapName +
                '.js"></script>';
            }

            jsfuncHtml =
              jsfuncHtml +
              "<script>\n//初始化地图\n" +
              "\t var myChart_" +
              key +
              " = echarts.init(document.getElementById('div" +
              key +
              "'),'customTheme');" +
              "\n" +
              "\t var option_" +
              key +
              " = " +
              JSON.stringify(optionMap[key]) +
              ";\n" +
              "\t for(let item of option_" +
              key +
              ".series ){\n" +
              "\t if(item.name == '散点' || item.name == '点' || item.name == 'Top 5'){\n" +
              "\t item.tooltip = { trigger: 'item',formatter:function (val) {\n" +
              "\t return val.name + ':' + val.value[2] + '" +
              element.suffix +
              "';}}\n" +
              "\t }else{item.tooltip = {}}}\n";

            for (var i in optionMap[key].series) {
              if (optionMap[key].series[i].name == "散点") {
                jsfuncHtml =
                  jsfuncHtml +
                  "\t option_" +
                  key +
                  ".series[" +
                  i +
                  "].symbolSize=function(val) {\n" +
                  "\t \t	return val[2] / 10;\n\t};\n";
              }
              if (optionMap[key].series[i].name == "点") {
                jsfuncHtml =
                  jsfuncHtml +
                  "\t option_" +
                  key +
                  ".series[" +
                  i +
                  "].symbolSize=function(val) {\n" +
                  "\t \t	var a = (50 - 20) / (500 - 10);\n" +
                  "\t \t	var b = 20 - a * 10;\n" +
                  "\t \t	b = 50 - a * 500;\n" +
                  "\t \t	return a * val[2] + b;\n};\n";
              }
            }

            jsfuncHtml =
              jsfuncHtml +
              "\t myChart_" +
              key +
              ".setOption(option_" +
              key +
              ", true);" +
              "\n" +
              "</script>\n";
          }
          //地图下钻
          else if (
            element.bindingType != null &&
            element.bindingType == "mapMore"
          ) {
            var provinces = {
              上海: "/codegen/json/data-1482909900836-H1BC_1WHg.json",
              河北: "/codegen/json/data-1482909799572-Hkgu_yWSg.json",
              山西: "/codegen/json/data-1482909909703-SyCA_JbSg.json",
              内蒙古: "/codegen/json/data-1482909841923-rkqqdyZSe.json",
              辽宁: "/codegen/json/data-1482909836074-rJV9O1-Hg.json",
              吉林: "/codegen/json/data-1482909832739-rJ-cdy-Hx.json",
              黑龙江: "/codegen/json/data-1482909803892-Hy4__J-Sx.json",
              江苏: "/codegen/json/data-1482909823260-HkDtOJZBx.json",
              浙江: "/codegen/json/data-1482909960637-rkZMYkZBx.json",
              安徽: "/codegen/json/data-1482909768458-HJlU_yWBe.json",
              福建: "/codegen/json/data-1478782908884-B1H6yezWe.json",
              江西: "/codegen/json/data-1482909827542-r12YOJWHe.json",
              山东: "/codegen/json/data-1482909892121-BJ3auk-Se.json",
              河南: "/codegen/json/data-1482909807135-SJPudkWre.json",
              湖北: "/codegen/json/data-1482909813213-Hy6u_kbrl.json",
              湖南: "/codegen/json/data-1482909818685-H17FOkZSl.json",
              广东: "/codegen/json/data-1482909784051-BJgwuy-Sl.json",
              广西: "/codegen/json/data-1482909787648-SyEPuJbSg.json",
              海南: "/codegen/json/data-1482909796480-H12P_J-Bg.json",
              四川: "/codegen/json/data-1482909931094-H17eKk-rg.json",
              贵州: "/codegen/json/data-1482909791334-Bkwvd1bBe.json",
              云南: "/codegen/json/data-1482909957601-HkA-FyWSx.json",
              西藏: "/codegen/json/data-1482927407942-SkOV6Qbrl.json",
              陕西: "/codegen/json/data-1482909918961-BJw1FyZHg.json",
              甘肃: "/codegen/json/data-1482909780863-r1aIdyWHl.json",
              青海: "/codegen/json/data-1482909853618-B1IiOyZSl.json",
              宁夏: "/codegen/json/data-1482909848690-HJWiuy-Bg.json",
              新疆: "/codegen/json/data-1482909952731-B1YZKkbBx.json",
              北京: "/codegen/json/data-1482818963027-Hko9SKJrg.json",
              天津: "/codegen/json/data-1482909944620-r1-WKyWHg.json",
              重庆: "/codegen/json/data-1482909775470-HJDIdk-Se.json",
              香港: "/codegen/json/data-1461584707906-r1hSmtsx.json",
              澳门: "/codegen/json/data-1482909771696-ByVIdJWBx.json",
              china: "/codegen/json/data-1527045631990-r1dZ0IM1X.json",
            };

            for (var i in provinces) {
              jsFolder.file(i + ".json", getBlob(provinces[i]));
            }

            //jsFolder.file("china.json", getBlob("./codegen/json/data-1527045631990-r1dZ0IM1X.json"));
            jsFolder.file("loadMap.js", getBlob("/codegen/js/loadMap.js"));
            jsLinkHtml =
              jsLinkHtml +
              '<script type="text/javascript" src="js/loadMap.js"></script>';

            jsfuncHtml =
              jsfuncHtml +
              "<script>\n//初始化地图下钻\n" +
              "\t var myChart_" +
              key +
              " = echarts.init(document.getElementById('div" +
              key +
              "'), 'customTheme');" +
              "\n" +
              "\t var option_" +
              key +
              " = {};\n" +
              "\t var dataOption =" +
              JSON.stringify(optionMap[key]) +
              ";\n" +
              "\t loadMap('js/china.json', 'china', myChart_" +
              key +
              ",dataOption);\n" +
              "\t var timeFn = null;" +
              "\n" +
              "\t var provinceName = ['上海','河北','山西','内蒙古','辽宁' ,'吉林' ,'黑龙江','江苏' ,'浙江', '安徽', '福建', '江西', '山东', '河南',\n" +
              "\t '湖北','湖南',  '广东', '广西', '海南', '四川', '贵州','云南', '西藏', '陕西', '甘肃', '青海', '宁夏', '新疆', '北京', '天津',\n" +
              "\t '重庆', '香港', '澳门']; \n" +
              "\t myChart_" +
              key +
              ".on('click', function(params) {" +
              "\n" +
              "\t \t	clearTimeout(timeFn);" +
              "\n" +
              "\t \t	timeFn = setTimeout(function() {" +
              "\n" +
              "\t \t	\t	var name = params.name;" +
              "\n" +
              '\t \t	\t	var mapCode ="js/"+name+".json";' +
              "\n" +
              "\t \t	\t	if (!mapCode || provinceName.indexOf(name) < 0) {return;}" +
              "\n" +
              "\t \t	\t	loadMap(mapCode, name, myChart_" +
              key +
              ", dataOption);" +
              "\n" +
              "\t \t	}, 250);" +
              "\n" +
              "\t });" +
              "\n" +
              "\t myChart_" +
              key +
              ".on('dblclick', function(params) {" +
              "\n" +
              "\t \t	clearTimeout(timeFn);" +
              "\n" +
              "\t \t	loadMap('js/china.json', 'china', myChart_" +
              key +
              ", dataOption);" +
              "\n" +
              "\t });" +
              "\n" +
              "\t myChart_" +
              key +
              ".setOption(option_" +
              key +
              ", true);" +
              "\n" +
              "</script>\n";
          }
          //3D地图
          else if (
            element.bindingType != null &&
            element.bindingType == "threedMap"
          ) {
            let mapName = element.series[0].map;
            if (mapName != "china") {
              jsFolder.file(
                mapName + ".js",
                getBlob("/codegen/js/province/" + mapName + ".js")
              );
              jsLinkHtml =
                jsLinkHtml +
                '<script type="text/javascript" src="js/' +
                mapName +
                '.js"></script>';
            }

            jsFolder.file(
              "echarts-gl.min.js",
              getBlob("/codegen/js/echarts-gl.min.js")
            );
            jsLinkHtml =
              jsLinkHtml +
              '<script type="text/javascript" src="js/echarts-gl.min.js"></script>';

            jsfuncHtml =
              jsfuncHtml +
              "<script>\n//初始化3D地图\n" +
              "\t var myChart_" +
              key +
              " = echarts.init(document.getElementById('div" +
              key +
              "'), 'customTheme');" +
              "\n" +
              "\t var option_" +
              key +
              " = " +
              JSON.stringify(optionMap[key]) +
              ";\n" +
              "\t myChart_" +
              key +
              ".setOption(option_" +
              key +
              ", true);" +
              "\n" +
              "</script>\n";
          }
          //3D地图柱状图
          else if (
            element.bindingType != null &&
            element.bindingType == "threedMapBar"
          ) {
            let mapName = element.geo3D.map;
            if (mapName != "china") {
              jsFolder.file(
                mapName + ".js",
                getBlob("/codegen/js/province/" + mapName + ".js")
              );
              jsLinkHtml =
                jsLinkHtml +
                '<script type="text/javascript" src="js/' +
                mapName +
                '.js"></script>';
            }

            jsFolder.file(
              "echarts-gl.min.js",
              getBlob("/codegen/js/echarts-gl.min.js")
            );
            jsLinkHtml =
              jsLinkHtml +
              '<script type="text/javascript" src="js/echarts-gl.min.js"></script>';

            jsfuncHtml =
              jsfuncHtml +
              "<script>\n//初始化3D地图\n" +
              "\t var myChart_" +
              key +
              " = echarts.init(document.getElementById('div" +
              key +
              "'), 'customTheme');" +
              "\n" +
              "\t var option_" +
              key +
              " = " +
              JSON.stringify(optionMap[key]) +
              ";\n" +
              "\t option_" +
              key +
              ".tooltip = { show: true,textStyle:{fontSize:20},formatter:function (params) {\n" +
              "\t return params.name + ':' + params.value[2] + '" +
              element.suffix +
              "';}}\n" +
              "\t option_" +
              key +
              ".series[0].label = { position:'top',distance:0, align:'center',textAlign:'center',show:" +
              element.labelShow +
              ",\n" +
              "\t textStyle:" +
              JSON.stringify(element.textStyle) +
              ",\n" +
              "\t formatter:function (params) {\n" +
              "\t return params.name + ':' + params.value[2] + '" +
              element.suffix +
              "';}}\n" +
              "\t myChart_" +
              key +
              ".setOption(option_" +
              key +
              ", true);" +
              "\n" +
              "</script>\n";
          }
          //地图-柱状图
          else if (
            element.bindingType != null &&
            element.bindingType == "mapbar"
          ) {
            jsfuncHtml =
              jsfuncHtml +
              "<script>\n//初始化地图-柱状图\n" +
              "\t var myChart_" +
              key +
              " = echarts.init(document.getElementById('div" +
              key +
              "'), 'customTheme');" +
              "\n" +
              "\t var option_" +
              key +
              " = " +
              JSON.stringify(optionMap[key]) +
              ";\n" +
              "\t myChart_" +
              key +
              ".setOption(option_" +
              key +
              ", true);" +
              "\n" +
              "\t myChart_" +
              key +
              ".on('brushselected', renderBrushed);" +
              "\n" +
              "\t var convertedData= option_" +
              key +
              ".series[0].data;" +
              "\n" +
              "\t function renderBrushed(params) {" +
              "\n" +
              "\t	\t	var mainSeries = params.batch[0].selected[0];" +
              "\n" +
              "\t \t	var selectedItems = [];" +
              "\n" +
              "\t \t	var categoryData = [];" +
              "\n" +
              "\t \t	var barData = [];" +
              "\n" +
              "\t \t	var maxBar = option_" +
              key +
              '.yAxis.name.replace("TOP ","");' +
              "\n" +
              "\t \t	var sum = 0;" +
              "\n" +
              "\t \t	var count = 0;" +
              "\n" +
              "\t \t	for (var i = 0; i < mainSeries.dataIndex.length; i++) {" +
              "\n" +
              "\t \t	\t	var rawIndex = mainSeries.dataIndex[i];" +
              "\n" +
              "\t \t	\t	var dataItem = convertedData[rawIndex];" +
              "\n" +
              "\t \t	\t	var pmValue = dataItem.value[2];" +
              "\n" +
              "\t \t	\t	sum += pmValue;" +
              "\n" +
              "\t \t	\t	count++;" +
              "\n" +
              "\t \t	\t	selectedItems.push(dataItem);" +
              "\n" +
              "\t \t	}" +
              "\n" +
              "\t \t	selectedItems.sort(function(a, b) {" +
              "\n" +
              "\t \t	\t	return b.value[2] - a.value[2];" +
              "\n" +
              "\t \t	});" +
              "\n" +
              "\t \t	for (var i = 0; i < Math.min(selectedItems.length, maxBar); i++) {" +
              "\n" +
              "\t \t	\t	categoryData.push(selectedItems[i].name);" +
              "\n" +
              "\t \t	\t	barData.push(selectedItems[i].value[2]);" +
              "\n" +
              "\t \t	}" +
              "\n" +
              "\t \t	this.setOption({" +
              "\n" +
              "\t \t	\t	yAxis: {" +
              "\n" +
              "\t \t	\t	\t	data: categoryData," +
              "\n" +
              "\t \t	\t	\t	inverse: true" +
              "\n" +
              "\t \t	\t	},xAxis: {" +
              "\n" +
              "\t \t	\t	\t	axisLabel: {" +
              "\n" +
              "\t \t	\t	\t	\t	show: !!count" +
              "\n" +
              "\t \t	\t	\t	}" +
              "\n" +
              "\t \t	\t	},title: {" +
              "\n" +
              "\t \t	\t	\t	id: 'statistic'," +
              "\n" +
              "\t \t	\t	\t	text: count ? '平均: ' + (sum / count).toFixed(0) : ''" +
              "\n" +
              "\t \t	\t	},series: {" +
              "\n" +
              "\t \t	\t	\t	id: 'bar'," +
              "\n" +
              "\t \t	\t	\t	data: barData" +
              "\n" +
              "\t \t	\t	}" +
              "\n" +
              "\t \t	});" +
              "\n" +
              "\t }" +
              "\n" +
              "</script>\n";
          }
          //飞线地图
          else if (
            element.bindingType != null &&
            element.bindingType == "mapline"
          ) {
            jsfuncHtml =
              jsfuncHtml +
              "<script>\n//初始化飞线地图\n" +
              "\t var myChart_" +
              key +
              " = echarts.init(document.getElementById('div" +
              key +
              "'),'customTheme');" +
              "\n" +
              "\t var option_" +
              key +
              " = " +
              JSON.stringify(optionMap[key]) +
              ";\n" +
              "\t option_" +
              key +
              ".tooltip = { trigger: 'item', showDelay: 0,hideDelay: 0,enterable: true,transitionDuration: 0,\n" +
              "\t extraCssText: 'z-index:100',formatter:function (params, ticket, callback) {\n" +
              "\t if(params.seriesType == 'effectScatter'){ \n" +
              "\t let name = params.name; \n" +
              "\t let value = params.value[params.seriesIndex + 1]; \n" +
              '\t let res = "<span style=\'color:#fff;font-size: 12px\'>  "+ name  + "："+ value \n' +
              "\t return res\n" +
              "\t   }\n" +
              "\t  }\n" +
              "\t }\n" +
              "\t let formatter = (e) => { return e.data.name }\n" +
              "\t  option_" +
              key +
              ".series[1].label.normal.formatter = formatter \n";

            jsfuncHtml =
              jsfuncHtml +
              "\t myChart_" +
              key +
              ".setOption(option_" +
              key +
              ", true);" +
              "\n" +
              "</script>\n";
          }
          //气泡图
          else if (
            element.bindingType != null &&
            element.bindingType == "pops"
          ) {
            jsfuncHtml =
              jsfuncHtml +
              "<script>\n//初始化图表\n" +
              "\t var myChart_" +
              key +
              " = echarts.init(document.getElementById('div" +
              key +
              "'));" +
              "\n" +
              "\t var option_" +
              key +
              " = " +
              JSON.stringify(optionMap[key]) +
              ";\n" +
              "\t myChart_" +
              key +
              ".setOption(option_" +
              key +
              ", true);" +
              "\n" +
              "</script>\n";
          }
          //横向立体柱状图
          else if (
            element.bindingType != null &&
            element.bindingType == "CustomThreeDBar"
          ) {
            jsfuncHtml =
              jsfuncHtml +
              "<script>\n//初始化横向立体柱状图\n" +
              "\t var myChart_" +
              key +
              " = echarts.init(document.getElementById('div" +
              key +
              "'),'customTheme');" +
              "\n" +
              "\t var setting = " +
              JSON.stringify(optionMap[key]) +
              ";\n" +
              "\t initRender(setting)" +
              ";\n" +
              "\t myChart_" +
              key +
              " .setOption(setting.option, true);" +
              "\n" +
              "</script>\n";

            // echarts-customThreeDBar.js依赖
            jsFolder.file(
              "echarts-customThreeDBar.js",
              getBlob("/codegen/js/echarts-customThreeDBar.js")
            );
            jsLinkHtml =
              jsLinkHtml +
              '<script type="text/javascript" src="js/echarts-customThreeDBar.js"></script>';
          } else if (
            element.bindingType != null &&
            element.bindingType == "pictorialBar"
          ) {
            jsfuncHtml =
              jsfuncHtml +
              "<script>\n//初始化象形图\n" +
              "\t var myChart_" +
              key +
              " = echarts.init(document.getElementById('div" +
              key +
              "'), 'customTheme');" +
              "\n" +
              "\t var option_" +
              key +
              " = " +
              JSON.stringify(optionMap[key]) +
              ";\n" +
              "\t if(option_" +
              key +
              ".symbolList.length>0){" +
              "\n" +
              "\t\t for(const item of option_" +
              key +
              ".option.series){" +
              "\n" +
              "\t\t\t for(const itemIndex in item.data){" +
              "\n" +
              "\t\t\t\t if(itemIndex < option_" +
              key +
              ".symbolList.length){" +
              "\n" +
              "\t\t\t\t\t item.data[itemIndex].symbol = " +
              "'" +
              "image://images/pictorialBar" +
              key +
              "-'+itemIndex+'.png'}" +
              "\n" +
              "\t\t\t\t else{item.data[itemIndex].symbol =" +
              "'" +
              "image://images/pictorialBar" +
              key +
              "-'+(option_" +
              key +
              ".symbolList.length-1) +'.png'}" +
              "\n" +
              "\t\t\t }" +
              "\n" +
              "\t\t }" +
              "\n" +
              "\t }" +
              "\n" +
              "\t myChart_" +
              key +
              ".setOption(option_" +
              key +
              ".option" +
              ", true);" +
              "\n" +
              "</script>\n";
          } else {
            jsfuncHtml =
              jsfuncHtml +
              "<script>\n//初始化图表\n" +
              "\t var myChart_" +
              key +
              " = echarts.init(document.getElementById('div" +
              key +
              "'), 'customTheme');" +
              "\n" +
              "\t var option_" +
              key +
              " = " +
              JSON.stringify(optionMap[key]) +
              ";\n" +
              "\t myChart_" +
              key +
              ".setOption(option_" +
              key +
              ", true);" +
              "\n" +
              "</script>\n";
          }
        }
      }

      bodyHtml = bodyHtml + "</div>";

      htmlStr =
        htmlStr +
        cssLinkHtml +
        jsLinkHtml +
        bodyHtml +
        jsfuncHtml +
        "</body></html>";
      zip.file("index.html", htmlStr);

      zip.generateAsync({ type: "blob" }).then((content) => {
        // 生成二进制流
        FileSaver.saveAs(content, response.data.screenName + ".zip"); // 利用file-saver保存文件  自定义文件名
      });
    }
  });
}

function getBlob(url) {
  return new Promise((resolve) => {
    const xhr = new XMLHttpRequest();
    xhr.open("GET", url, true);
    xhr.responseType = "blob";
    xhr.onload = () => {
      if (xhr.status === 200) {
        resolve(xhr.response);
      }
    };
    xhr.send();
  });
}
