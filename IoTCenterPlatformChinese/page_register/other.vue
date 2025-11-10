<template>
	<view>
		
	</view>
</template>

<script>
	export default {
		data() {
			return {
				
			}
		},
		onLoad(options) {
			if(options.path){
				uni.switchTab({
					url:options.path
				})
			}else{
				uni.switchTab({
					url:'/pages/profile/profile'
				})
			}
			
		},
		methods: {
			
		}
	}
</script>

<style>

</style>
