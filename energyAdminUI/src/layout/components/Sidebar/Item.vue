<script>
export default {
  name: "MenuItem",
  functional: true,
  props: {
    icon: {
      type: String,
      default: ""
    },
    title: {
      type: String,
      default: ""
    },
    isAvtive: {
      type: Boolean,
      default: false
    },
    isNest: {
      type: Boolean,
      default: false
    },
  },
  computed: {
    iconName() {
      return `#icon-${this.name}`;
    },
  },
  render(h, context) {
    console.log();
    const { icon, title, isAvtive,isNest } = context.props;
    const vnodes = [];
    // let className = "";
    let iconname='zhongtaiiconfont zhongtai-icon-'+icon
    if (context.props.isAvtive) {
      // className = "activeIcon";
      iconname=iconname+' activeIcon'
    }

    if (icon) {
      // console.log("context.computed.style", { className });

      // vnodes.push(<svg-icon icon-class={icon} class={className} />);
      // console.log(icon,'iconiconicon');
      if(isNest){
        vnodes.push(<span style="padding-left:4px;"></span>);
      }else{
        if(isAvtive){
          vnodes.push(<i class={iconname} style="margin-right: 10px;color:rgba(255, 255, 255, 1);"></i>);
        }else{
          vnodes.push(<i class={iconname} style="margin-right: 10px;color:rgba(255, 255, 255, 0.6);"></i>);
        }
        
      }
      
    }else{
      vnodes.push(<span style="padding-left:4px;"></span>);
    }

    if (title) {
      vnodes.push(<span slot="title">{title}</span>);
    }
    return vnodes;
  }
};
</script>
<style lang="scss" scope>
// @import "~@/assets/icons/customIcon/iconfont.css";

.activeIcon {
  // fill:#6aa5ff !important;
  color: rgba(255, 255, 255, 0.6) !important;
}
</style>
