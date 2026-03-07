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
    return {
		styles: {
			color: '#333',
			backgroundColor: 'rgba(248, 248, 248, 1)',
			disableColor: '#F8F8F8',
			borderColor: 'rgba(234, 234, 234, 1)',
		},
	}
  },
  
  computed: {
    _value: {
      get() {
		  // console.log("有回写数据吗",this.value);
        return this.value;
      },
      set(val) {
		// console.log("修改设置值",val);
		this.$emit('update:value',val)
        this.$emit("input", val);
      }
    }
  },
}
