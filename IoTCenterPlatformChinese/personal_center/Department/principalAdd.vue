<template>
	<view>
		<top leftIcon="icon-fanhui" :isleftBack="true"  backgroundColor="#ffffff"
			title="设置部门负责人" class="addPool-top"></top>
		<view class="uni-list">
			<checkbox-group @change="checkboxChange">
				<label class="uni-list-cell uni-list-cell-pd" v-for="(item, index) in peopleList" :key="index">
					<view>
						<checkbox :value="item.Id.toString()" :checked="item.checked" />
					</view>
					<view class="peopleInfo">
						<image class="avatarImg" :src="item.Avatar" mode=""></image>
						<view>{{item.RealName}}</view>
					</view>
				</label>
			</checkbox-group>
		</view>
		<view class="btn">
			<view class="btnInfo">
				<button type="primary" @click="settingPeople">设置负责人</button>
			    <button type="warn" @click="deptMove">移动至部门</button>
			</view>
		</view>
	</view>
</template>

<script>
import { listMember, setLeaders } from "@/api/personalCenter";
export default {
	data() {
		return {
			deptId: '',
			peopleList: [],
			uids: []
		}
	},
	onLoad(option) {
		if (option.deptId) {
			this.deptId = option.deptId;
		}
	},
	onShow() {
		this.getPeople();
	},
	methods: {
		getPeople() {
			listMember({ deptId: this.deptId, deptIdWithChildren: false,isPrimaryDept:true }).then(res => {
				if(res.code === 0) {
					res.data.List.forEach(element => {
						if (element.IsLeader) {
							element.checked = true
							this.uids.push(element.Id);
						}
					});
					this.peopleList = res.data.List;
				}
			})
		},
		checkboxChange(e) {
			this.uids = e.detail.value
		},
		settingPeople() {
			if(this.uids.length > 0) {
				setLeaders({ depId: this.deptId, leaders: this.uids }).then(res => {
					if(res.code === 0) {
						uni.showToast({
							title: '设置成功',
							icon:'success',
							duration: 2000
						})
					} else {
						uni.showToast({
							title: res.message,
							icon: 'none',
							duration: 2000
						})
					}
				})
			} else {
				uni.showToast({
                    title: '请选择负责人',
                    icon: 'none',
                    duration: 2000
                })
                return;
			}
		},
		deptMove() {
			if(this.uids.length > 0) {
				let ids = JSON.stringify(this.uids)
				uni.navigateTo({
				    url: `/personal_center/Department/departmentMove?uids=${ids}&deptId=${this.deptId}`
				})
			} else {
				uni.showToast({
				    title: '请选择要移动的人员',
				    icon: 'none',
				    duration: 2000
				})
				return;
			}
			
		}
	}
}
</script>
<style lang="less" scoped>
page{
	background: #ffffff;
}
.uni-list{
	padding: 30rpx;
}
.uni-list-cell {
	display: flex;
	align-items: center;
	justify-content: flex-start;
	margin-bottom: 25rpx;
}
.peopleInfo{
	display: flex;
	align-items: center;
}
.avatarImg{
	width: 90rpx;
	height: 90rpx;
	border-radius: 50%;
	margin: 0 25rpx;
}
.btn{
	position: fixed;
	bottom: 0;
	left: 0;
	width: 100%;
}
.btnInfo{
	display: flex;
	justify-content: space-around;
	padding: 20rpx;
}
.btn button{
	width: 48%;
}
</style>