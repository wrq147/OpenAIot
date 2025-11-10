<template>
	<!--渲染表单-->
	<!-- <el-form ref="form" class="process-form" label-position="top" :rules="rules" :model="_value">
		<el-form-item v-if="item.name !== 'SpanLayout' && item.name !== 'Description'" :prop="item.id"
			:label="item.title" v-for="(item, index) in forms" :key="item.name + index">
			<form-design-render :ref="`sub-item_${item.id}`" v-model="_value[item.id]" :valueModel="value" mode="PC"
				:config="item" />
		</el-form-item>
		<form-design-render ref="span-layout" v-else v-model="_value" mode="PC" :valueModel="value" :config="item" />
	</el-form> -->
	<view>
		<uni-forms ref="form" :modelValue="_value" :rules="rules" labelWidth='80' label-position="top">
			<view class="form_con pages_form no_radius">
				<view v-for="(item, index) in forms" :key="item.name + index" v-if="forms&&forms.length>0">
				 <!-- :id="'form_'+item.id" -->
					<uni-forms-item
						v-if="item.name !== 'SpanLayout'" :label="item.title"
						:name="item.id" labelFont="34rpx" contentFont="32rpx"
						:requireOpacity="0.5" labelPosition="top" :isDis="item.props&&item.props.disabled||disabled" :required="item.props&&item.props.required">
						<view :id="'li_'+forms[index].id" class="form_li">
							<form-design-render @update:value="updateval1($event,forms[index].id,forms[index])" style="width: 100%;" :ref="'sub-item_'+item.id" :value="_value[forms[index].id]" :valueModel="value"
								mode="PC" :config="forms[index]" :keyId="forms[index].id" @setActiveItem='setActiveItem' :disabled="item.props.disabled||disabled"/>
						</view>
					</uni-forms-item>
					<view :id="'li_'+item.id" class="form_li" :class="{'dis_form_li':item.props&&item.props.disabled||disabled}" v-else>
						<form-design-render @update:value="updateval" style="width: 100%;" :ref="'sub-item_'+item.id" :value="_value" :valueModel="value"
							mode="PC" :config="item" :keyId="item.id" @setActiveItem="setActiveItem" :isShowFirshBorder="index==0" :disabled="item.props.disabled||disabled"/>
					</view>
				</view>
			</view>
		</uni-forms>
		<msg-prompt ref="promptMsg"></msg-prompt>
	</view>
</template>

<script>
	import FormDesignRender from './form-design-render.vue'

	export default {
		name: "FormRender",
		components: {
			FormDesignRender
		},
		props: {
			forms: {
				type: Array,
				default: () => {
					return []
				}
			},
			optionInit: {
				type: Array,
				default: () => {
					return []
				}
			},
			value: {
				type: Object,
				default: () => {
					return {}
				}
			},
			disabled: {
				default: false,
				type: Boolean,
			},
		},
		data() {
			return {
				rules: {},
				isFirst:true,
				optionInitIdList:[],//操作初始化表单id
			}
		},
		updated() {
			if(this.isFirst){
				this.loadFormConfig(this.forms)
				this.isFirst=false
			}
		},
		computed: {
			_value: {
				get() {
					return this.value
				},
				set(val) {
					// this.$emit('update:value',val)
					// this.$emit('input', val)
				}
			}
		},
		methods: {
			updateval1(val,id,item){
				let value=JSON.parse(JSON.stringify(this._value))
				value[item.id]=val
				this.$emit('update:value',value)
			},
			updateval(val){
				this.$emit('update:value',val)
				// this.$emit('input', val)
			},
			selectDept(val, acitveId) {
				//选择部门
				this.$refs[`sub-item_${acitveId}`][0].selectDept(val, acitveId)
			},
			selectEmplee(val, acitveId) {
				//选择人员
				this.$refs[`sub-item_${acitveId}`][0].selectEmplee(val, acitveId)
			},
			selectDevice(val, acitveId) {
				//选择设备
				this.$refs[`sub-item_${acitveId}`][0].selectDevice(val, acitveId)
			},
			setActiveItem(val) {
				this.$emit('setActiveItem', val)
			},
			validate(call,ac) {
				let success = true
				this.$refs.form.validate().then(async valid => {
					success = valid
					if (valid) {
						//校验成功再校验内部
						for (let i = 0; i < this.forms.length; i++) {
							if (this.forms[i].name === 'TableList') {
								let formRef = this.$refs[`sub-item_${this.forms[i].id}`]
								if (formRef && Array.isArray(formRef) && formRef.length > 0) {
									// formRef[0].validate().then(subValid => {
									// 	console.log(subValid,'subValid');
										
									// }).catch(er=>{
										
									// })
									try{
										let subValid=await formRef[0].validate()
										success = subValid
									}catch(er){
										//TODO handle the exception
										success=false
										if (er && er.length > 0) {
											let firstErr = er[0].key
											// #ifdef MP-WEIXIN
											const query = uni.createSelectorQuery().in(this);
											let str="#li_"+firstErr
											query.select(str).boundingClientRect(data => {
												uni.pageScrollTo({
													scrollTop: data.top - 200,
													// selector: firstErr,
													duration: 300
												});
										
											}).exec();
											// #endif
											// #ifndef MP-WEIXIN 
											uni.pageScrollTo({
												selector: '#form_'+firstErr,
												duration: 300
											});
											// #endif  
										}
									}
									if (!success) {
										break
									}
								}
							}else{
								if(this.optionInitIdList.includes(this.forms[i].id)){//操作填写初始化提示
									if(this.forms[i].props.required&&this._value[this.forms[i].id]==''||this.forms[i].props.required&&this._value[this.forms[i].id]==null){
										let acname=this.optionInit.find(rws=>rws.fieldid==this.forms[i].id)
										if(acname&&acname.optionName !==ac){
											this.$refs.promptMsg.open('请输入'+this.forms[i].title, 3000)
											success=false
											break
										}
										
									}
								}
							}
						}
					}
					call(success)
				}).catch(err=>{
					call(false)
					if (err && err.length > 0) {
						let firstErr = err[0].key
						// #ifdef MP-WEIXIN
						const query = uni.createSelectorQuery().in(this);
						let str="#li_"+firstErr
						query.select(str).boundingClientRect(data => {
							uni.pageScrollTo({
								scrollTop: data.top - 200,
								// selector: firstErr,
								duration: 300
							});
					
						}).exec();
						// #endif
						// #ifndef MP-WEIXIN 
						uni.pageScrollTo({
							selector: '#form_'+firstErr,
							duration: 300
						});
						// #endif  
					}
				});
			},
			loadFormConfig(forms) {
				if(this.optionInit){
					this.optionInitIdList=this.optionInit.map(row=>row.fieldid)
				}
				
				forms.map(item => {
					if (item.name === 'SpanLayout') {
						this.loadFormConfig(item.props.items)
					} else {
						this.$set(this._value, item.id, this.value[item.id])
						this.$emit('update:value',this._value)
						// if(item.name === 'TableList'&&item.props&&item.props.rowLayout){
						// 	console.log(item,'itemitemitemitem');
						// 	this.loadFormConfig(item.props.columns)
						// }
						if (!item.props.disabled&&item.props.required) {
							if(this.optionInitIdList&&this.optionInitIdList.includes(item.id)){}else{
								this.$set(this.rules, item.id, {rules:[{
									type: item.valueType === 'Array' ? 'array' : undefined,
									required: true,
									errorMessage: `请填写${item.title}`,
									// trigger: 'blur'
								}]})
							}
							
						}
					}
				})
				// console.log(this.rules,'this.rulesthis.rulesthis.rules',this.optionInitIdList);
			}
		}
	}
</script>

<style lang="less">
	.select_popup {
		position: fixed;
		left: 100%;
		bottom: 100%;
		z-index: 2;
	}
</style>