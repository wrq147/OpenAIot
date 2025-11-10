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
			color: '#fff',
			backgroundColor: '#161A26',
			disableColor: 'rgba(28, 34, 50, 1)',
			borderColor: 'rgba(255, 255, 255, 0.20)'
		},
	}
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
