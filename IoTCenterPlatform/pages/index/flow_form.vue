<template>
	<view>
		<uni-forms ref="loginForm" :modelValue="flowForm" :rules="rules" labelWidth='80' label-position="top">
			<view class="form_con pages_form">
				<uni-forms-item label="Single line input" required name="username" id="username_form" labelFont="32rpx"
					contentFont="32rpx" :requireOpacity="0.5" labelPosition="top" :rules="username.rules" :isDis="true"
					:isfirstTop="true">
					<view id="username" class="form_li">
						<!-- <uni-easyinput placeholderStyle="color:rgba(255, 255, 255, 0.2);font-size:32rpx"
							inputHeight="88rpx" :styles="styles" type="text" v-model="flowForm.value"
							placeholder="Please enter content" contentFontSize="32rpx"
							primaryColor="rgba(255, 255, 255, 0.5)" /> -->
						<view class="dis_text">
							Please enter content
						</view>
					</view>
				</uni-forms-item>
				<uni-forms-item label="Notes" name="Remark" id="Remark_form" labelFont="32rpx" contentFont="32rpx"
					:requireOpacity="0.5" :isDis="true">
					<view id="Remark" class="form_li">
						<!-- <uni-easyinput placeholderStyle="color:rgba(255, 255, 255, 0.2);font-size:32rpx"
							inputHeight="70rpx" :styles="styles" type="textarea" v-model="flowForm.Remark"
							placeholder="Please enter the notes" contentFontSize="32rpx"
							primaryColor="rgba(255, 255, 255, 0.5)" autoHeight /> -->
						<view class="dis_text">
							Please enter content please enter content ple-
							-ase enter content
						</view>
					</view>
				</uni-forms-item>
				<uni-forms-item label="Single line input" required name="username" id="username_form" labelFont="32rpx"
					contentFont="32rpx" :requireOpacity="0.5" labelPosition="top" :rules="username.rules" :isDis="true"
					:isLastTop="true">
					<view id="username" class="form_li">
						<view style="width: 100%;">
							<view class="li_flex">
								<input class="li_input" type="digit" placeholder="Please enter content"
									placeholderStyle="color:rgba(255, 255, 255, 0.2);font-size:32rpx"
									v-model="flowForm.number" @input="filterValue2($event,6,'number')" />
									<!-- filterValue($event,'number') -->
								<view class="num_cli">
									<view class="cli_up" @click.stop="numUp('number')">
										<custom-icons iconsName="icon-shuzijiajiansanjiao" iconsSize="14rpx"
											iconsColor="rgba(255, 255, 255, 0.5)"></custom-icons>
									</view>
									<view class="cli_up rotate-180" @click.stop="numDown('number')">
										<custom-icons iconsName="icon-shuzijiajiansanjiao" iconsSize="14rpx"
											:iconsColor="flowForm.number>0?'rgba(255, 255, 255, 0.5)':'rgba(255, 255, 255, 0.2)'"></custom-icons>
									</view>
								</view>
							</view>
							<view class="dis_text" style="margin-top: 15rpx" v-show="showChinese">
								<text>大写：</text>
								<text class="chinese">{{ chinese }}</text>
							</view>
						</view>
					</view>
				</uni-forms-item>
				<uni-forms-item label="Single line input" required name="username" id="username_form" labelFont="32rpx"
					contentFont="32rpx" :requireOpacity="0.5" labelPosition="top" :rules="username.rules">
					<view id="username" class="form_li">
						<input class="li_input input" type="digit" placeholder="Please enter content"
							placeholderStyle="color:rgba(255, 255, 255, 0.2);font-size:32rpx" v-model="flowForm.float"
							@input="filterValue2($event,3,'float')" />
						<view class="num_cli">
							<view class="cli_up" @click.stop="numUp('float')">
								<custom-icons iconsName="icon-shuzijiajiansanjiao" iconsSize="14rpx"
									iconsColor="rgba(255, 255, 255, 0.5)"></custom-icons>
							</view>
							<view class="cli_up rotate-180" @click.stop="numDown('float')">
								<custom-icons iconsName="icon-shuzijiajiansanjiao" iconsSize="14rpx"
									:iconsColor="flowForm.float>0?'rgba(255, 255, 255, 0.5)':'rgba(255, 255, 255, 0.2)'"></custom-icons>
							</view>
						</view>
					</view>
				</uni-forms-item>
				<uni-forms-item label="Single line input" name="username" id="username_form" labelFont="32rpx"
					contentFont="32rpx" :requireOpacity="0.5" labelPosition="top">
					<view id="username" class="form_li">
						<uni-data-select v-model="flowForm.select" :localdata="localdata" width="100%"
							placeholder="Please select a department" borderColor="rgba(255, 255, 255, 0.20)"
							palColor="rgba(255, 255, 255, 0.2)" :isCustom="true" :isDark="true"></uni-data-select>
					</view>
				</uni-forms-item>
				<uni-forms-item label="Single line input" name="username" id="username_form" labelFont="32rpx"
					contentFont="32rpx" :requireOpacity="0.5" labelPosition="top">
					<view id="username" class="form_li">
						<uni-data-checkbox v-model="flowForm.select" :localdata="localdata" selectedColor="#FF3535"
							selectedTextColor="#ffffff" custextColor="#ffffff" disabledColor="rgba(255, 255, 255, 0.5)"
							noselectedColor="rgba(255, 255, 255, 0.5)" :isColumn="true" textFont="32rpx"
							textLeftMargin="16rpx"></uni-data-checkbox>
					</view>
				</uni-forms-item>
				<uni-forms-item label="Multiple choice" name="username" id="username_form" labelFont="32rpx"
					contentFont="32rpx" :requireOpacity="0.5" labelPosition="top">
					<view id="username" class="form_li">
						<view class="view_input" @click="toOpen">
							<view class="pal_col" v-if="!mulForm[mulActiveLabel]||mulForm[mulActiveLabel].length==0">
								请选择
							</view>
							<view class="view_li_con" id="view_li_mul"
								:class="{'shenglue_li':mulArr.includes('multipleSelect')}">
								<view class="view_li_cot" id="multipleSelect_li">
									<view class="view_li" v-for="(item,inx) in mulForm[mulActiveLabel]">
										<view class="view_text">
											{{item.text}}
										</view>
										<view class="view_icon" @click.stop="delMulLi(item,inx,'multipleSelect')">
											<custom-icons iconsName="icon-guanbidanchuang" iconsSize="20rpx"
												iconsColor="rgba(255, 255, 255, 1)"></custom-icons>
										</view>
									</view>
								</view>
							</view>
							<view class="view_mask"></view>
							<view class="form_sel_icon">
								<custom-icons iconsName="icon-xialajiantou" iconsSize="14rpx"
									iconsColor="rgba(255, 255, 255, 0.5)"></custom-icons>
							</view>
						</view>
					</view>
				</uni-forms-item>
				<uni-forms-item label="Multiple choice" name="username" id="username_form" labelFont="32rpx"
					contentFont="32rpx" :requireOpacity="0.5" labelPosition="top">
					<view id="username" class="form_li">
						<uni-data-checkbox v-model="flowForm.multipleSelect" multiple :localdata="localdata"
							selectedColor="#FF3535" selectedTextColor="#ffffff" custextColor="#ffffff"
							disabledColor="rgba(255, 255, 255, 0.5)" noselectedColor="rgba(255, 255, 255, 0.5)"
							:isColumn="true" textFont="32rpx" textLeftMargin="16rpx"
							noSelectBorder="rgba(255, 255, 255, 0.5)" noSelectBg="inherit" :isRadius="true"
							@change="multipleSelectChange('multipleSelect')"></uni-data-checkbox>
					</view>
				</uni-forms-item>
				<uni-forms-item label="Date Time Point" name="username" id="username_form" labelFont="32rpx"
					contentFont="32rpx" :requireOpacity="0.5" labelPosition="top">
					<view id="username" class="form_li">
						<view class="int_date">
							<view class="date_con long_date" style="color: #fff;">
								<uni-datetime-picker ref="dateChoice1" class="date" type="datetime"
									placeholder-style="font-size:32rpx;color:#999999" :clear-icon="false"
									v-model="flowForm.datetime" placeholder='开始日期' :isCustom="true"
									@change='dateChange1' :isDark="true">
									<view class="date_slot" :class="{'has_val':flowForm.datetime}">
										<view class="text">
											{{flowForm.datetime?flowForm.datetime:'Start date'}}
										</view>
										<custom-icons iconsName="icon-xuanzeshijian" iconsSize="28rpx"
											iconsColor="rgba(255, 255, 255, 0.5)"></custom-icons>
									</view>
								</uni-datetime-picker>
							</view>
						</view>
					</view>
				</uni-forms-item>
				<uni-forms-item label="Date time interval" name="username" id="username_form" labelFont="32rpx"
					contentFont="32rpx" :requireOpacity="0.5" labelPosition="top">
					<view id="username" class="form_li">
						<view class="int_date">
							<view class="date_con long_date" style="color: #fff;">
								<uni-datetime-picker v-model="flowForm.date" type="daterange" :isDark="true"
									:isCustom="true" />
							</view>
						</view>
					</view>
				</uni-forms-item>
				<uni-forms-item label="Upload pictures" name="username" id="username_form" labelFont="32rpx"
					contentFont="32rpx" :requireOpacity="0.5" labelPosition="top">
					<view id="username" class="form_li">
						<view class="">
							<view class="image_con">
								<view class="image_li" v-for="(item,inx) in imagesList">
									<image class="image" :src="item+'?wh=500x500'" mode="aspectFit"></image>
									<view class="del_icon" @click.stop="delImg(inx,imagesList)">
										<view class="icons_del t-icon-yichu"></view>
									</view>
								</view>
								<view class="image_li" @click.stop="uploadImg(imagesList)">
									<view class="image_icon t-icon-shangchuantupian"></view>
								</view>
							</view>
							<view class="tips_text">
								Select images, each image should not exceed 10MB
							</view>
						</view>
					</view>
				</uni-forms-item>
				<uni-forms-item label="Upload attachments" name="username" id="username_form" labelFont="32rpx"
					contentFont="32rpx" :requireOpacity="0.5" labelPosition="top">
					<view id="username" class="form_li">
						<view class="">
							<view class="file_con">
								<view class="file_li" v-for="(item,inx) in flowForm.fileList">
									<view class="file_icon t-icon-wenjian"></view>
									<view class="file_name">
										{{item.name}}
									</view>
									<view class="del_icon" @click.stop="handleDeleteFile(inx,flowForm.fileList)">
										<view class="icons_del t-icon-yichu"></view>
									</view>
								</view>
								<view class="file_li" @click.stop="handleUploadClick('file',['pdf'])">
									<view class="file_icon t-icon-fujian"></view>
									<view class="file_name pal">
										Select file
									</view>
								</view>
							</view>
							<view class="tips_text">
								Please select a file. Only files in the format of [txt, ppt,
								doc. xls. pdf] are allowed to be uploaded, and a single
								attachment cannot exceed 10MB
							</view>
							<xe-upload ref="XeUpload" :options="uploadOptions"
								@callback="handleUploadCallback($event,flowForm.fileList)"></xe-upload>
						</view>
					</view>
				</uni-forms-item>
				<uni-forms-item label="Select personnel" name="username" id="username_form" labelFont="32rpx"
					contentFont="32rpx" :requireOpacity="0.5" labelPosition="top">
					<view id="username" class="form_li">
						<view class="view_input" @click="choiceManage">
							<view class="pal_col" v-if="!flowForm.Leader||flowForm.Leader.length==0">
								{{isMulPersons?'Please select personnels':'Please select a personnel'}}
							</view>
							<view class="view_li_con personnel_li_con" v-if="flowForm.Leader&&flowForm.Leader.length>0">
								<view class="view_li_cot personnel_li_cot">
									<view class="view_li" v-for="(item,inx) in flowForm.Leader">
										<view class="view_text">
											{{item.name}}
										</view>
										<view class="view_icon" @click.stop="delMulPer(inx,flowForm.Leader)">
											<custom-icons iconsName="icon-guanbidanchuang" iconsSize="20rpx"
												iconsColor="rgba(255, 255, 255, 1)"></custom-icons>
										</view>
									</view>
								</view>
							</view>
							<view class="view_mask"></view>
							<view class="form_sel_icon">
								<custom-icons iconsName="icon-a-youjiantoubai" iconsSize="20rpx"
									iconsColor="rgba(255, 255, 255, 0.5)"></custom-icons>
							</view>
						</view>
					</view>
				</uni-forms-item>
				<uni-forms-item label="Select device" name="username" id="username_form" labelFont="32rpx"
					contentFont="32rpx" :requireOpacity="0.5" labelPosition="top">
					<view id="username" class="form_li">
						<view class="form_device_list">
							<view class="form_device_li" v-for="(item,inx) in selectDeviceList" :key="item.Id">
								<view class="li_left">
									<image class="image" :src="item.PhotoUrl+'?wh=500x500'" mode=""></image>
								</view>
								<view class="li_right">
									<view class="name">{{item.Name}}</view>
									<view class="number">{{item.DeviceNumber}}</view>
								</view>
								<view class="del_icon" @click.stop="delSelDev(inx)">
									<view class="icons_del t-icon-yichu"></view>
								</view>
							</view>
							<view class="form_device_add" @click="toChoiceDevice">
								<custom-icons iconsName="icon-tianjia" iconsSize="24rpx"
									iconsColor="rgba(255, 255, 255, 0.5)"></custom-icons>
								<view class="text">
									Add item
								</view>
							</view>
						</view>
					</view>
				</uni-forms-item>
				<uni-forms-item label="Select department" name="department" id="department_form" labelFont="32rpx"
					contentFont="32rpx" :requireOpacity="0.5" labelPosition="top">
					<view id="department" class="form_li">
						<view class="view_input" @click="choiceDepartment">
							<view class="pal_col" v-if="!flowForm.dept||flowForm.dept.length==0">
								{{isMulDept?'Please select departments':'Please select a department'}}
							</view>
							<view class="view_li_con personnel_li_con" v-if="flowForm.dept&&flowForm.dept.length>0">
								<view class="view_li_cot personnel_li_cot">
									<view class="view_li" v-for="(item,inx) in flowForm.dept">
										<view class="view_text">
											{{item.name}}
										</view>
										<view class="view_icon" @click.stop="delMulPer(inx,flowForm.dept)">
											<custom-icons iconsName="icon-guanbidanchuang" iconsSize="20rpx"
												iconsColor="rgba(255, 255, 255, 1)"></custom-icons>
										</view>
									</view>
								</view>
							</view>
							<view class="view_mask"></view>
							<view class="form_sel_icon">
								<custom-icons iconsName="icon-a-youjiantoubai" iconsSize="20rpx"
									iconsColor="rgba(255, 255, 255, 0.5)"></custom-icons>
							</view>
						</view>
					</view>
				</uni-forms-item>
				<view id="text" class="form_li">
					<text class="form_text">
						<text class="iconfont #icon-icon icon-baojing"
							style="color:rgba(255, 255, 255, 0.2);font-size:32rpx;margin-right: 8rpx;"></text>Explanation
						text content explanation text content explanation text content explanation text content
					</text>
				</view>
				<uni-forms-item label="Schedule" name="text" id="text_form" labelFont="32rpx" contentFont="32rpx"
					:requireOpacity="0.5" labelPosition="top">
					<view id="text" class="form_li">
						<view class="table_con">
							<view class="table_li">
								<uni-forms-item label="Notes" name="Remark" id="Remark_form" labelFont="32rpx"
									contentFont="32rpx" :requireOpacity="0.5">
									<view id="Remark" class="form_li">
										<uni-easyinput placeholderStyle="color:rgba(255, 255, 255, 0.2);font-size:32rpx"
											inputHeight="70rpx" :styles="styles" type="textarea"
											v-model="flowForm.Remark" placeholder="Please enter the notes"
											contentFontSize="32rpx" primaryColor="rgba(255, 255, 255, 0.5)"
											autoHeight />
									</view>
								</uni-forms-item>
								<uni-forms-item label="Single line input" required name="username" id="username_form"
									labelFont="32rpx" contentFont="32rpx" :requireOpacity="0.5" labelPosition="top"
									:rules="username.rules">
									<view id="username" class="form_li">
										<input class="li_input" type="number" placeholder="Please enter content"
											placeholderStyle="color:rgba(255, 255, 255, 0.2);font-size:32rpx"
											v-model="flowForm.number" @input="filterValue($event,3,'number')" />
										<view class="num_cli">
											<view class="cli_up" @click.stop="numUp('number')">
												<custom-icons iconsName="icon-shuzijiajiansanjiao" iconsSize="14rpx"
													iconsColor="rgba(255, 255, 255, 0.5)"></custom-icons>
											</view>
											<view class="cli_up rotate-180" @click.stop="numDown('number')">
												<custom-icons iconsName="icon-shuzijiajiansanjiao" iconsSize="14rpx"
													:iconsColor="flowForm.number>0?'rgba(255, 255, 255, 0.5)':'rgba(255, 255, 255, 0.2)'"></custom-icons>
											</view>
										</view>
									</view>
								</uni-forms-item>
								<view class="table_btn">
									<view class="btn_li">Copy</view>
									<view class="btn_line"></view>
									<view class="btn_li">Delete</view>
								</view>
							</view>
							<view class="form_device_add">
								<custom-icons iconsName="icon-tianjia" iconsSize="24rpx"
									iconsColor="rgba(255, 255, 255, 0.5)"></custom-icons>
								<view class="text">
									Add item
								</view>
							</view>
						</view>
					</view>
				</uni-forms-item>
				<uni-forms-item label="Associated form" name="department" id="department_form" labelFont="32rpx"
					contentFont="32rpx" :requireOpacity="0.5" labelPosition="top">
					<view id="department" class="form_li">
						<view class="view_input">
							<view class="pal_col">
								Please select associated form
							</view>
							<view class="form_sel_icon">
								<custom-icons iconsName="icon-a-youjiantoubai" iconsSize="20rpx"
									iconsColor="rgba(255, 255, 255, 0.5)"></custom-icons>
							</view>
						</view>
					</view>
				</uni-forms-item>
				<button class="submit_button" @click="submit">
					Log on
				</button>
			</view>
		</uni-forms>
		<view class="select_popup">
			<jp-select-plus ref="selectPlus" label="多选带搜索-拼接展示结果" placeholder="请选择" isJoin checkbox
				v-model="mulForm[mulActiveLabel]" :list="localdata" keys="value" showName="text"
				@toConfirm="getMultipleVal" color="#FF3535" checkboxColor="#ffffff"></jp-select-plus>
		</view>
		<msg-prompt ref="promptMsg"></msg-prompt>
	</view>
</template>

<script>
	import {
		uploadPhoto,
		delPhoto,
	} from '@/api/user.js'
	import serverUrl from '@/common/constVar.js'
	import {
		getToken
	} from '@/common/auth.js'
	import {
		numberToWords
	} from '@/common/toenglish.js'
	export default {
		data() {
			return {
				isMulDevice: false, //是否是设备多选
				isMulPersons: true, //是否是人员多选
				isMulDept: true, //是否是部门多选
				selectDeviceList: [], //已经选择的设备列表
				fileList: [],
				uploadOptions: { //上传文件参数
					// url: '', // 不传入上传地址则返回本地链接
					// headers: {
					// 	Authorization: getToken(),
					// },
				},
				imagesList: [
					'https://52.28.35.196/uploads/2023/1220/a6883b3e730a4419976c6b97ea7e56ab.png',
					'https://52.28.35.196/uploads/2023/1214/c3680f286eac4ec18ade7f6a7f336fa1.png',
					'https://52.28.35.196/uploads/2023/1214/628f7e2685204d7383a61442e179173a.jpg',
					'https://52.28.35.196/uploads/2023/1214/a98ed6c8c8fd4624a606fe7afc3e6283.jpg',
					'https://52.28.35.196/uploads/2023/1214/c05556a8cffd4a85b6ef025a6a1e0642.jpg',
				],
				mulForm: {

				},
				mulActiveLabel: '', //多选时指定字段
				username: {
					rules: [{
						required: true,
						errorMessage: 'Please enter an account',
					}]
				},
				flowForm: {
					number: 0,
					float: 0,
					multipleSelect: [],
					date: [null, null],
					fileList: [{
						url: 'https://52.28.35.196/uploads/2023/1220/4f422fb4751841bfa0e318ab0862c06c.txt',
						name: '11111.txt'
					}, {
						url: 'https://52.28.35.196/uploads/2023/1220/9fc92d38c7dc4e09814b5d38273e0b5a.txt',
						name: '备份.txt'
					}, {
						url: 'https://52.28.35.196/uploads/2023/1215/369751008595417caea5cd386974e487.pdf',
						name: '2022-4-2-16-27-45.pdf',
					}, {
						url: 'https://52.28.35.196/uploads/2023/1215/013f42bddc484c9194868ea580f3f7fe.txt',
						name: '备份.txt'
					}]
				},
				rules: {},
				value: '',
				styles: {
					color: '#fff',
					backgroundColor: '#161A26',
					disableColor: 'rgba(28, 34, 50, 1)',
					borderColor: 'rgba(255, 255, 255, 0.20)'
				},
				localdata: [{
						text: "danxuan1",
						value: "0"
					},
					{
						text: "danxuan2",
						value: "1"
					},
					{
						text: "danxuan3",
						value: "2"
					}, {
						text: "danxuan43333rrrrrrrrrrrrrr",
						value: "3"
					},
					{
						text: "danxuan5",
						value: "4"
					},
					{
						text: "danxuan6",
						value: "5"
					}
				],
				mulArr: [], //控制多选框是否展示省略号
				showChinese: true
			};
		},
		mounted() {
			this.setMulVal('multipleSelect')
			// this.uploadOptions.url = serverUrl.getServerUrl() + '/AuthService/File/Upload?withDomain=true'
		},
		computed: {
			chinese() {
				return this.convertCurrency(this.flowForm.number);
			},
		},
		methods: {
			convertCurrency(money) {
				//转化
				
				let chineseStr = "";
				// chineseStr=numberToWords(money, "en")+' yuan'
				chineseStr=numberToWords(money)
				return chineseStr;
			},
			delSelDev(inx) {
				this.selectDeviceList.splice(inx, 1)
			},
			toChoiceDevice() {
				let select = JSON.stringify(this.selectDeviceList)
				if (this.isMulDevice) {
					uni.navigateTo({
						url: '/pages_device/select_list?selectDevice=' + select + '&isMulSelect=' + this
							.isMulDevice
					})
				} else {
					uni.navigateTo({
						url: '/pages_device/select_list?selectDevice=' + select
					})
				}
			},
			selectDevice(val) {
				//选中设备后
				console.log("设备", val);
				if (val) {
					this.selectDeviceList = val
					this.$forceUpdate()
				}

			},
			delMulPer(inx, list) {
				list.splice(inx, 1)
				this.$forceUpdate()
			},
			selectDept(val) {
				//选中负责人后
				console.log("负责人", val);
				if (val) {
					this.flowForm.dept = val
					this.$forceUpdate()
				}

			},
			selectEmplee(val) {
				//选中负责人后
				console.log("负责人", val);
				if (val) {
					this.flowForm.Leader = val
					this.$forceUpdate()
				}

			},
			choiceManage() {
				//选择员工
				let selected = this.flowForm.Leader ? JSON.stringify(this.flowForm.Leader) : JSON.stringify([])
				if (this.isMulPersons) {
					uni.navigateTo({
						url: '/pages_Inventory/employee_select?selected=' + selected + '&multiple=' + this
							.isMulPersons
					})
				} else {
					uni.navigateTo({
						url: '/pages_Inventory/employee_select?selected=' + selected
					})
				}
			},
			choiceDepartment() {
				//选择部门
				let selected = this.flowForm.dept ? JSON.stringify(this.flowForm.dept) : JSON.stringify([])
				if (this.isMulDept) {
					uni.navigateTo({
						url: '/pages_Inventory/employee_select?selected=' + selected + '&multiple=' + this
							.isMulDept + '&type=dept'
					})
				} else {
					uni.navigateTo({
						url: '/pages_Inventory/employee_select?selected=' + selected + '&type=dept'
					})
				}
			},
			handleUploadClick(type, fileType) {
				console.log("iiiiii", type, fileType);
				let fileArr = []
				fileType.map(row => {
					// #ifndef H5
					let str = ''
					if (row.indexOf('txt') > -1) {
						str = 'text/plain'
					}
					if (row.indexOf('ppt') > -1) {
						str = 'application/vnd.ms-powerpoint'
					}
					if (row.indexOf('doc') > -1) {
						str = 'application/msword'
					}
					if (row.indexOf('xls') > -1) {
						str = 'application/vnd.ms-excel'
					}
					if (row.indexOf('pdf') > -1) {
						str = 'application/pdf'
					}
					// #endif
					// #ifdef H5
					let str = '.' + row
					// #endif
					fileArr.push(str)
				})
				// 使用默认配置则不需要传入第二个参数
				// App、H5 文件拓展名过滤 { extension: ['.doc', '.docx'] } 或者 { extension: '.doc, .docx' }
				this.$refs.XeUpload.upload(type, {
					extension: fileArr
				});
			},
			async handleUploadCallback(e, fileList) {
				console.log('UploadCallback', e);
				if (['choose', 'success'].includes(e.type)) {
					// 根据接口返回修改对应的response相关的逻辑
					for (let i = 0; i < (e.data || []).length; i++) {
						let {
							response,
							tempFilePath,
							size,
							name,
							fileType
						} = e.data[i]
						if (size && size > 10 * 1024 * 1024) {
							this.$refs.promptMsg.open('Uploading images cannot exceed 10MB', 2000)
							return;
						}
						const res = response?.result || {};
						const tmpUrl = res.filePath ?? tempFilePath;
						const tmpName = res.fileName ?? name;
						let fileUpUrl = await uploadPhoto(tmpUrl)
						console.log("上传后", fileUpUrl);
						let tmpFiles = {
							...res,
							url: fileUpUrl,
							name: tmpName,
							fileType,
						};
						console.log(tmpFiles, 'yyyyyyy上传的');
						fileList.push(tmpFiles);
					}
				}
			},
			async handleDeleteFile(inx, fileList) {
				//删除文件
				this.$refs.promptMsg.loadingOpen('deleting...')
				try {
					console.log("fileList", fileList[inx]);
					let rsp2 = await delPhoto(fileList[inx].url)
					fileList = fileList.splice(inx, 1)
					this.$refs.promptMsg.loadingColse()
				} catch (err) {
					//TODO handle the exception
					this.$refs.promptMsg.loadingColse()
					this.setMsgTop(err)
				}
				fileList.splice(inx, 1);
			},
			async delImg(inx, imagesList) {
				//删除图片
				this.$refs.promptMsg.loadingOpen('deleting...')
				try {
					let rsp2 = await delPhoto(imagesList[inx])
					imagesList = imagesList.splice(inx, 1)
					this.$refs.promptMsg.loadingColse()
				} catch (err) {
					//TODO handle the exception
					this.$refs.promptMsg.loadingColse()
					this.setMsgTop(err)
				}

			},
			uploadImg(imagesList) {
				//手动上传图片
				uni.chooseImage({
					count: 5,
					sizeType: ['compressed'], //可以指定是原图还是压缩图，默认二者都有
					success: async (res) => {
						this.$refs.promptMsg.loadingOpen('Uploading...')
						if (res.tempFilePaths.length == 0) {
							return;
						}
						for (let i = 0; i < res.tempFiles.length; i++) {
							if (res.tempFiles[i].size > 10 * 1024 * 1024) {
								this.$refs.promptMsg.open('Uploading images cannot exceed 10MB', 2000)
								return;
							}
							try {
								let paths = res.tempFilePaths[i];
								let rsp = await uploadPhoto(paths)
								console.log("rsp图片结果", rsp);
								imagesList.push(rsp)
								console.log("图片列表", this.imagesList);
								this.$refs.promptMsg.loadingColse()
							} catch (e) {
								//TODO handle the exception
								this.$refs.promptMsg.loadingColse()
								// console.log(e);
								this.setMsgTop(e)
							}
						}
					}
				})
			},
			dateChange1() {
				//日期改变1

			},
			dateChange2() {
				//日期改变1

			},
			delMulLi(row, inx, label) {
				//多选时删除元素
				console.log(this.mulForm[label], this.flowForm[label]);
				this.mulForm[label].splice(inx, 1)
				this.flowForm[label].splice(inx, 1)
				this.$nextTick(() => {
					let conWid = 0
					const query = uni.createSelectorQuery().in(this);
					query.select('#view_li_mul').boundingClientRect(data1 => {
						conWid = data1.width
					})
					query.select('#' + this.mulActiveLabel + '_li').boundingClientRect(data => {
						if (data.width > conWid) {
							if (this.mulArr.includes(this.mulActiveLabel)) {} else {
								this.mulArr.push(this.mulActiveLabel)
							}
						} else {
							console.log(this.mulArr, this.mulArr.includes(this.mulActiveLabel));
							if (this.mulArr.includes(this.mulActiveLabel)) {
								this.mulArr = this.mulArr.filter(item => {
									item != this.mulActiveLabel
								})
							}
						}
					}).exec();
				})
			},
			setMulVal(label) {
				//设置多选时
				this.mulActiveLabel = label
				this.mulForm[label] = []

			},
			getMultipleVal(val) {
				//获取多选值
				this.mulForm[this.mulActiveLabel] = val
				console.log(this.mulForm[this.mulActiveLabel], 'this.mulForm[this.mulActiveLabel]');
				let arr = []
				val.map(row => {
					arr.push(row.value)
				})
				this.flowForm.multipleSelect = arr
				this.$nextTick(() => {
					let conWid = 0
					const query = uni.createSelectorQuery().in(this);
					query.select('#view_li_mul').boundingClientRect(data1 => {
						conWid = data1.width
					})
					query.select('#' + this.mulActiveLabel + '_li').boundingClientRect(data => {
						if (data.width > conWid) {
							if (this.mulArr.includes(this.mulActiveLabel)) {} else {
								this.mulArr.push(this.mulActiveLabel)
							}
						} else {
							console.log(this.mulArr, this.mulArr.includes(this.mulActiveLabel));
							if (this.mulArr.includes(this.mulActiveLabel)) {
								this.mulArr = this.mulArr.filter(item => {
									item != this.mulActiveLabel
								})
							}
						}
					}).exec();
				})
			},
			toOpen() { //打开多选弹窗
				this.$refs.selectPlus.open()
			},
			multipleSelectChange(label) {
				//多选数据改变
				this.$nextTick(() => {
					console.log(this.flowForm[label], '多选');
				})
			},
			numUp(label) {
				this.$nextTick(() => {
					if (String(this.flowForm[label]).indexOf('.') > -1) {
						let arr = String(this.flowForm[label]).split('.')
						this.flowForm[label] = Number(String(Number(arr[0]) + 1) + '.' + arr[1])
					} else {
						this.flowForm[label] = Number(this.flowForm[label]) + 1
					}
				})
			},
			numDown(label) {
				this.$nextTick(() => {
					if (this.flowForm[label] <= 0 || this.flowForm[label] - 1 <= 0) {
						this.flowForm[label] = 0
						return
					}
					if (String(this.flowForm[label]).indexOf('.') > -1) {
						let arr = String(this.flowForm[label]).split('.')
						this.flowForm[label] = Number(String(Number(arr[0]) - 1) + '.' + arr[1])
					} else {
						this.flowForm[label] = Number(this.flowForm[label]) - 1
					}

				})
			},
			filterValue(input, label) {
				//限制只允许输入数字
				if (n > 0) {
					let value = input.target.value;
					value = value.replace(/[^\d.]/g, '');
					value = value.replace(/^\./g, '');
					value = value.replace('.', '$#$').replace('$#$', '.');
					if (n && Number(n) > 0) { //限制n位
						var d = new Array(Number(n)).fill(`\\d`).join('');
						var reg = new RegExp(`^(\\-)*(\\d+)\\.(${d}).*$`, 'ig');
						value = value.replace(reg, '$1$2.$3')
					}
					if (value && !value.includes('.')) {
						value = Number(value).toString() //去掉开头多个0
					}
					this.$nextTick(() => {
						this.flowForm[label] = value
					})
				} else {
					let value = input.target.value;
					value = value.replace(/\D+/g, '');
					input.value = value ? Number(value).toString() : value //去掉开头多个0
					this.$nextTick(() => {
						this.flowForm[label] = value
					})
				}
			},
			filterValue2(input, n, label) {
				//实现保留指定小数位
				console.log(Number(1.20), 'number');
				if (n > 0) {
					let value = input.target.value;
					value = value.replace(/[^\d.]/g, '');
					value = value.replace(/^\./g, '');
					value = value.replace('.', '$#$').replace('$#$', '.');
					if (n && Number(n) > 0) { //限制n位
						var d = new Array(Number(n)).fill(`\\d`).join('');
						var reg = new RegExp(`^(\\-)*(\\d+)\\.(${d}).*$`, 'ig');
						value = value.replace(reg, '$1$2.$3')
					}
					if (value && !value.includes('.')) {
						value = Number(value).toString() //去掉开头多个0
					}
					this.$nextTick(() => {
						this.flowForm[label] = value
					})
				} else {
					let value = input.target.value;
					value = value.replace(/\D+/g, '');
					input.value = value ? Number(value).toString() : value //去掉开头多个0
					this.$nextTick(() => {
						this.flowForm[label] = value
					})
				}

			},
			submit() {
				this.$refs.loginForm.validate().then(res => {
					console.log(res, 'res');
				}).catch((e) => {
					console.log("eeee", e);
				})
			}
		}
	}
</script>

<style lang="less" scoped>
	.select_popup {
		position: fixed;
		left: 100%;
		bottom: 100%;
		z-index: 2;
	}

	
</style>