//混入组件数据
export default{
  props:{
    mode:{
      type: String,
      default: 'DESIGN'
    },
    valueModel:{
      type: Object,
      default: () => {
        return {}
      }
    },
    required:{
      type: Boolean,
      default: false
    },
  },
  data(){
    return {}
  },
  computed: {
    _value: {
      get() {
        return this.value;
      },
      set(val) {
        this.$emit("input", val);
      }
    }
  },
}
