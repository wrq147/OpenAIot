<template>
	<view class="l-circle" :class="[{clockwise: !clockwise && !useCanvas}, ['is-' + lineCap]]" :style="[styles]">
		<!-- #ifndef APP-NVUE -->
		<view class="check"></view>
		<view v-if="!useCanvas" class="l-circle__trail" :style="[trailStyles]">
			<text class="cap start"></text>
			<text class="cap end"></text>
		</view>
		<view v-if="!useCanvas" class="l-circle__stroke" :style="[strokeStyles]">
			<view class="l-circle__stroke-line"></view>
			<text class="cap start" v-if="current"></text>
			<text class="cap end" v-if="current"></text>
		</view>
		<canvas v-if="useCanvas" type="2d" :canvas-id="canvasId" :id="canvasId" class="l-circle__canvas" ></canvas>
		<!-- #endif -->
		<!-- #ifdef APP-NVUE -->
		<web-view 
			@pagefinish="finished = true" 
			@error="onerror"
			@onPostMessage="onMessage"
			class="l-circle__view" 
			ref="webview"
			src="/uni_modules/lime-circle/hybrid/html/index.html"></web-view>
		<!-- #endif -->
		<view class="l-circle__inner">
			<slot></slot>
		</view>
	</view>
</template>

<script lang="ts">
	// @ts-nocheck
	import { ref, computed, watch, reactive, defineComponent, onMounted ,onUnmounted, getCurrentInstance, nextTick } from './vue';
	import { useTransition } from './useTransition';
	import CircleProps from './props';
	import { getCanvas , isCanvas2d} from './getCanvas';
	import {Circle} from './circle'
	import { addUnit } from '@/uni_modules/lime-shared/addUnit';
	import { unitConvert } from '@/uni_modules/lime-shared/unitConvert';
	import { isString } from '@/uni_modules/lime-shared/isString';
	import { getRect } from '@/uni_modules/lime-shared/getRect';
	// import { useTransition } from '@/uni_modules/lime-use';
	
	export default defineComponent({
		name: 'l-circle',
		props: CircleProps,
		emits: ['update:current'],
		setup(props, {emit}) {
			const context = getCurrentInstance()
			const useCanvas = ref(props.canvas)
			const canvasId = `l-circle-${context.uid}`;
			let circleCanvas = null
			
			const RADIAN = Math.PI / 180
			const ratio = computed(() => 100 / props.max)
			const percent = ref<number>(0)
			const angle = computed(() => props.dashboard ? 135 : -90)
			const isShowCap = computed(() => {
				const { dashboard } = props
				return current.value > 0 && (dashboard ? true : current.value < props.max)
			})

			const offsetTop = ref<number | string>(0)
			const strokeEndCap = reactive({
				x: '0',
				y: '0'
			})

			const styles = computed(() => ({
				width: addUnit(props.size),
				height:addUnit(props.size),
				// #ifdef APP-NVUE
				transform: !useCanvas.value && `translateY(${offsetTop.value})`,
				// #endif
				// #ifndef APP-NVUE
				'--l-circle-offset-top':  !useCanvas.value && offsetTop.value,
				// #endif
			}))
			const classes = computed(() => {
				const { clockwise, lineCap } = props
				// {
				// 	clockwise: !clockwise && !useCanvas.value,
				// 	[`is-${lineCap}`]: lineCap
				// }
				return lineCap ? `is-${lineCap} ` : ' ' + !clockwise && !useCanvas.value && `clockwise`
			})
			
			// css render 
			const trailStyles = computed(() => {
				const { size, trailWidth, trailColor, dashboard } = props
				const circle = getCircle(size, trailWidth)
				const mask = `radial-gradient(transparent ${circle.r - 0.5}px, #000 ${circle.r}px)`

				let background = ''
				let capStart = { x: '', y: '' }
				let capEnd = capStart

				if (dashboard) {
					background = `conic-gradient(from 225deg, ${trailColor} 0%, ${trailColor} 75%, transparent 75%, transparent 100%)`
					capStart = calcPosition(circle.c, 135)
					capEnd = calcPosition(circle.c, 45)
					offsetTop.value = (unitConvert(size) - (unitConvert(capStart.y) + unitConvert(trailWidth) / 2)) / 4 + 'px'
				} else {
					background = `${trailColor}`
				}

				return {
					color: trailColor,
					mask,
					'-webkit-mask': mask,
					background,
					'--l-circle-trail-cap-start-x': capStart.x,
					'--l-circle-trail-cap-start-y': capStart.y,
					'--l-circle-trail-cap-end-x': capEnd.x,
					'--l-circle-trail-cap-end-y': capEnd.y,
					// '--l-circle-trail-cap-color': trailColor,
					'--l-circle-trail-cap-size': addUnit(unitConvert(trailWidth))
				}
			})
			const strokeStyles = computed(() => {
				const { size, strokeWidth, strokeColor, dashboard, max } = props
				const circle = getCircle(size, strokeWidth)
				const percent = dashboard ? current.value * 0.75 * ratio.value : current.value * ratio.value;
				const mask = `radial-gradient(transparent ${circle.r - 0.5}px, #000 ${circle.r}px)`
				const cap = calcPosition(circle.c, angle.value)

				let startColor = '';
				let endColor = '';
				let gradient = `conic-gradient(${dashboard ? 'from 225deg,' : ''} transparent 0%,`;
				let gradientEnd = `transparent var(--l-circle-percent), transparent ${dashboard ? '75%' : '100%'})`

				if (isString(strokeColor)) {
					gradient += ` ${strokeColor} 0%, ${strokeColor} var(--l-circle-percent), ${gradientEnd}`
					startColor = endColor = strokeColor
				} else if (Array.isArray(strokeColor)){
					const len = strokeColor.length
					for (let i = 0; i < len; i++) {
						const color = strokeColor[i] as string
						if (i === 0) {
							gradient += `${color} 0%,`
							startColor = color
						} else {
							gradient += `${color} calc(var(--l-circle-percent) * ${(i + 1) / len}),`
						}
						if (i == len - 1) {
							endColor = color
						}
					}
					gradient += gradientEnd
				} 
				return {
					mask,
					'-webkit-mask': mask,
					'--l-background': gradient,
					// background: isString(strokeColor) ? strokeColor : strokeColor[0],
					// background: gradient,
					// transition: `--l-circle-percent ${duration}ms`,
					'--l-circle-percent': `${percent / ratio.value == max ? percent + 0.1 : percent}%`,
					'--l-circle-stroke-cap-start-color': startColor,
					'--l-circle-stroke-cap-end-color': endColor,
					'--l-circle-stroke-cap-start-x': cap.x,
					'--l-circle-stroke-cap-start-y': cap.y,
					'--l-circle-stroke-cap-end-x': strokeEndCap.x,
					'--l-circle-stroke-cap-end-y': strokeEndCap.y,
					'--l-circle-stroke-cap-size': addUnit(unitConvert(strokeWidth)),
					'--l-circle-stroke-cap-opacity': isShowCap.value ? 1 : 0
				}
			})
			const calcStrokeCap = () => {
				const { size, strokeWidth, dashboard, max } = props
				const circle = getCircle(size, strokeWidth)
				const arc = dashboard ? 180 / 2 * 3 : 180 * 2
				const step = arc / max * current.value + angle.value
				const cap = calcPosition(circle.c, step)

				strokeEndCap.x = cap.x
				strokeEndCap.y = cap.y
			}

			const calcPosition = (r : number, angle : number) => {
				return {
					x: r + r * Math.cos(angle * RADIAN) + 'px',
					y: r + r * Math.sin(angle * RADIAN) + 'px'
				}
			}

			const getCircle = (size : number | string, lineWidth : number | string) => {
				const s = unitConvert(size)
				const w = unitConvert(lineWidth)
				const c = (s - w) / 2
				const r = s / 2 - w
				return {
					s, w, c, r
				}
			}
			// css render end
			const [current, stopTransition] = useTransition(percent, {
				duration: props.duration,
			})
			const stopPercent = watch(() => props.percent, (v) => {
				percent.value = v
				circleCanvas && circleCanvas.play(v)
			})
			
			const stopCurrent = watch(current, (v) => {
				if(!useCanvas.value) {
					calcStrokeCap()
				} 
				emit('update:current', v.toFixed(2))
			})
			const getProps = () => {
				const {strokeWidth, trailWidth} = props
				return Object.assign({}, props, {trailWidth: unitConvert(trailWidth), strokeWidth: unitConvert(strokeWidth)})
			}
			// #ifdef APP-NVUE
			const finished = ref(false)
			const init = ref(false)
			const webview = ref(null)
			const onerror = () => {
				
			}
			const onMessage = (e: any) => {
				const {detail:{data: [res]}} = e;
				if(res.event == 'init') {
					useCanvas.value = res.data.useCanvas // && props.canvas
					init.value = true;
					circleCanvas = {
						setOption(props: any) {
							webview.value.evalJs(`setOption(${JSON.stringify(props)})`)
						},
						play(v: number) {
							webview.value.evalJs(`play(${v})`)
						}
					}
				}
				if(res.event == 'progress') {
					current.value = res.data
				}
			}
			let stopFinnished = watch(init, () => {
				stopFinnished()
				if(useCanvas.value) {
					circleCanvas.setOption(getProps())
					circleCanvas.play(props.percent)
					stopTransition()
				} else {
					webview.value.evalJs(`setClass('.l-circle', 'is-round', ${props.lineCap == 'round'})`)
					webview.value.evalJs(`setClass('.l-circle', 'clockwise', ${props.clockwise})`)
					stopFinnished = watch([trailStyles, strokeStyles], (v) => {
						webview.value.evalJs(`setStyle(0,${JSON.stringify(v[0])})`)
						webview.value.evalJs(`setStyle(1,${JSON.stringify(v[1])})`)
					}, { immediate: true })
				}
				percent.value = props.percent
			})
			
			// #endif
			// #ifndef APP-NVUE
			onMounted(() => {
				getRect('.check', {context}).then(res => {
					// alert(CSS.supports('background', 'conic-gradient(#000, #fff)'))
					useCanvas.value = !(res.height > 0 && !props.canvas)
					if(useCanvas.value) {
						stopTransition()
						setTimeout(() => {
							getCanvas(canvasId, {context}).then(res => {
								circleCanvas = new Circle(res, {
									size: unitConvert(props.size),
									run: (v: number) => current.value = v,
									pixelRatio: isCanvas2d ? uni.getSystemInfoSync().pixelRatio : 1,
								})
								circleCanvas.setOption(getProps())
								circleCanvas.play(props.percent)
							})
						},50)
					}
					percent.value = props.percent
				})
			})
			// #endif
			onUnmounted(() => {
				stopPercent()
				stopCurrent()
				stopTransition()
				// #ifdef APP-NVUE
				stopFinnished && stopFinnished()
				// #endif
			})
			return {
				useCanvas,
				canvasId,
				classes,
				styles,
				trailStyles,
				strokeStyles,
				current,
				// #ifdef APP-NVUE
				webview,
				onerror,
				onMessage,
				finished
				// #endif
			}
		}
	})
</script>
<style lang="scss">
	@import './index';
</style>