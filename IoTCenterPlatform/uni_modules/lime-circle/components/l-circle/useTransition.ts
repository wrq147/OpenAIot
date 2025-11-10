// @ts-nocheck
import {ease} from './animation/ease';
import {Timeline, Animation} from './animation/';
import {ref, watch, Ref} from './vue'
	
export function useTransition(percent: Ref<number>, options: {duration: number}) {
	const current = ref(0)
	const {immediate, duration} = options
	let tl = new Timeline();
	
	const stop = watch(() => percent.value, (v) => {
		tl.start();
		tl.add(
		  new Animation(
		    current.value,
		    v,
		    duration,
		    0,
		    ease,
		    v => {
				current.value = v < 0.0001 ? 0: v
			}
		  )
		);
	}, {immediate})
	
	return [current, stop]
}