<template>
	<view style="width: 100%;">
		<!-- <el-form :rules="rules" :model="row" :ref="`table-form-${i}`" class="table-column"
				v-for="(row, i) in _value" :key="i">
				<div class="table-column-action">
					<span>第 {{ i + 1 }} 项</span>
					<i v-if="!disabled" class="el-icon-close" @click="delRow(i, row)"></i>
				</div>
				<el-form-item v-for="(column, index) in _columns" :key="'column_' + index" :prop="column.id"
					:label="column.title">
					<form-design-render v-model="row[column.id]" :mode="mode" :config="column" />
				</el-form-item>
			</el-form>
			<el-button size="small" icon="el-icon-plus" v-if="!disabled" @click="addRow">{{ placeholder }}</el-button> -->

		<!-- <div v-if="showSummary">
				<div class="sumcls">合计：{{totalval}}{{summaryUnit}}</div>
			</div> -->

		<!-- <view class="form_con pages_form"> -->

		<view class="table_con" v-if="disabled&&rowLayout">
			<!-- <uni-forms :ref="`table-form-${i}`" :modelValue="row" :rules="rules" labelWidth='80' label-position="top"
				 :key="i"> -->
			<view class="" v-for="(row, i) in _value">
				<view class="table_li">
					<uni-forms-item :required="column.props&&column.props.required" :label="column.title"
						:name="column.id" :id="'form_'+column.id+'_'+i+index" labelFont="32rpx" contentFont="32rpx"
						:requireOpacity="0.5" v-for="(column, index) in _columns" :key="'column_' + index"
						:isDis="column.props&&column.props.disabled" :isfirstTop="index==0">
						<view :id="'li_'+column.id+'_'+i+index" class="form_li">
							<TextInput :ref="'form'+column.id" :is="column.name" :mode="mode" v-model="row[column.id]"
								v-bind="column.props" :keyId="column.id" v-if="column&&column.name=='TextInput'">
							</TextInput>
							<NumberInput :ref="'form'+column.id" :is="column.name" :mode="mode" v-model="row[column.id]"
								v-bind="column.props" :keyId="column.id" v-if="column&&column.name=='NumberInput'">
							</NumberInput>
							<AmountInput :ref="'form'+column.id" :is="column.name" :mode="mode" v-model="row[column.id]"
								v-bind="column.props" :keyId="column.id" v-if="column&&column.name=='AmountInput'">
							</AmountInput>
							<TextareaInput :ref="'form'+column.id" :is="column.name" :mode="mode"
								v-model="row[column.id]" v-bind="column.props" :keyId="column.id"
								v-if="column&&column.name=='TextareaInput'"></TextareaInput>
							<SelectInput :ref="'form'+column.id" :is="column.name" :mode="mode" v-model="row[column.id]"
								v-bind="column.props" :keyId="column.id" v-if="column&&column.name=='SelectInput'">
							</SelectInput>
							<MultipleSelect :ref="'form'+column.id" :is="column.name" :mode="mode"
								v-model="row[column.id]" v-bind="column.props" :keyId="column.id"
								v-if="column&&column.name=='MultipleSelect'"></MultipleSelect>
							<DateTime :ref="'form'+column.id" :is="column.name" :mode="mode" v-model="row[column.id]"
								v-bind="column.props" :keyId="column.id" v-if="column&&column.name=='DateTime'">
							</DateTime>
							<DateTimeRange :ref="'form'+column.id" :is="column.name" :mode="mode"
								v-model="row[column.id]" v-bind="column.props" :keyId="column.id"
								v-if="column&&column.name=='DateTimeRange'"></DateTimeRange>
							<Description :ref="'form'+column.id" :is="column.name" :mode="mode" v-model="row[column.id]"
								v-bind="column.props" :keyId="column.id" v-if="column&&column.name=='Description'">
							</Description>
							<ImageUpload :ref="'form'+column.id" :is="column.name" :mode="mode" v-model="row[column.id]"
								v-bind="column.props" :keyId="column.id" v-if="column&&column.name=='ImageUpload'">
							</ImageUpload>
							<FileUpload :ref="'form'+column.id" :is="column.name" :mode="mode" v-model="row[column.id]"
								v-bind="column.props" :keyId="column.id" v-if="column&&column.name=='FileUpload'">
							</FileUpload>
							<Location :ref="'form'+column.id" :is="column.name" :mode="mode" v-model="row[column.id]"
								v-bind="column.props" :keyId="column.id" v-if="column&&column.name=='Location'">
							</Location>
							<DeptPicker :ref="'form'+column.id" :is="column.name" :mode="mode" v-model="row[column.id]"
								v-bind="column.props" :keyId="column.id" @setActiveItem="setActiveItem"
								v-if="column&&column.name=='DeptPicker'"></DeptPicker>
							<UserPicker :ref="'form'+column.id" :is="column.name" :mode="mode" v-model="row[column.id]"
								v-bind="column.props" :keyId="column.id" @setActiveItem="setActiveItem"
								v-if="column&&column.name=='UserPicker'"></UserPicker>
							<DevicPicker :ref="'form'+column.id" :is="column.name" :mode="mode" v-model="row[column.id]"
								v-bind="column.props" :keyId="column.id" @setActiveItem="setActiveItem"
								v-if="column&&column.name=='DevicPicker'"></DevicPicker>
							<SignPanel :ref="'form'+column.id" :is="column.name" :mode="mode" v-model="row[column.id]"
								v-bind="column.props" :keyId="column.id" @setActiveItem="setActiveItem"
								v-if="column&&column.name=='SignPanel'"></SignPanel>
							<SpanLayout :ref="'form'+column.id" :is="column.name" :mode="mode" v-model="_value"
								v-bind="column.props" :keyId="column.id" @setActiveItem="setActiveItem"
								v-if="column&&column.name=='SpanLayout'"></SpanLayout>
							<TableList :ref="'form'+column.id" :is="column.name" :mode="mode" v-model="row[column.id]"
								v-bind="column.props" :keyId="column.id" @setActiveItem="setActiveItem"
								v-if="column&&column.name=='TableList'"></TableList>
							<ParamInput :ref="'form'+column.id" :is="column.name" :mode="mode" v-model="row[column.id]"
								v-bind="column.props" :keyId="column.id" @setActiveItem="setActiveItem"
								v-if="column&&column.name=='ParamInput'"></ParamInput>
						</view>
					</uni-forms-item>
					<view class="sort_num">
						{{i + 1}}
					</view>
				</view>
			</view>

			<!-- </uni-forms> -->
		</view>
		<view class="table_con" v-else-if="disabled&&!rowLayout">
			<uni-forms :ref="`table-form-${i}`" :modelValue="row" :rules="rules" labelWidth='80' label-position="top"
				v-for="(row, i) in _value" :key="'table'+i">
				<view class="table_li">
					<uni-forms-item :required="column.props&&column.props.required" :label="column.title"
						:name="column.id" :id="'form_'+column.id+'_'+i+index" labelFont="32rpx" contentFont="32rpx"
						:requireOpacity="0.5" v-for="(column, index) in _columns" :key="'column_' + index"
						:isDis="column.props&&column.props.disabled" :isfirstTop="index==0">
						<view :id="'li_'+column.id+'_'+i+index" class="form_li">
							<TextInput :ref="'form'+column.id" :is="column.name" :mode="mode" v-model="row[column.id]"
								v-bind="column.props" :keyId="column.id" v-if="column&&column.name=='TextInput'">
							</TextInput>
							<NumberInput :ref="'form'+column.id" :is="column.name" :mode="mode" v-model="row[column.id]"
								v-bind="column.props" :keyId="column.id" v-if="column&&column.name=='NumberInput'">
							</NumberInput>
							<AmountInput :ref="'form'+column.id" :is="column.name" :mode="mode" v-model="row[column.id]"
								v-bind="column.props" :keyId="column.id" v-if="column&&column.name=='AmountInput'">
							</AmountInput>
							<TextareaInput :ref="'form'+column.id" :is="column.name" :mode="mode"
								v-model="row[column.id]" v-bind="column.props" :keyId="column.id"
								v-if="column&&column.name=='TextareaInput'"></TextareaInput>
							<SelectInput :ref="'form'+column.id" :is="column.name" :mode="mode" v-model="row[column.id]"
								v-bind="column.props" :keyId="column.id" v-if="column&&column.name=='SelectInput'">
							</SelectInput>
							<MultipleSelect :ref="'form'+column.id" :is="column.name" :mode="mode"
								v-model="row[column.id]" v-bind="column.props" :keyId="column.id"
								v-if="column&&column.name=='MultipleSelect'"></MultipleSelect>
							<DateTime :ref="'form'+column.id" :is="column.name" :mode="mode" v-model="row[column.id]"
								v-bind="column.props" :keyId="column.id" v-if="column&&column.name=='DateTime'">
							</DateTime>
							<DateTimeRange :ref="'form'+column.id" :is="column.name" :mode="mode"
								v-model="row[column.id]" v-bind="column.props" :keyId="column.id"
								v-if="column&&column.name=='DateTimeRange'"></DateTimeRange>
							<Description :ref="'form'+column.id" :is="column.name" :mode="mode" v-model="row[column.id]"
								v-bind="column.props" :keyId="column.id" v-if="column&&column.name=='Description'">
							</Description>
							<ImageUpload :ref="'form'+column.id" :is="column.name" :mode="mode" v-model="row[column.id]"
								v-bind="column.props" :keyId="column.id" v-if="column&&column.name=='ImageUpload'">
							</ImageUpload>
							<FileUpload :ref="'form'+column.id" :is="column.name" :mode="mode" v-model="row[column.id]"
								v-bind="column.props" :keyId="column.id" v-if="column&&column.name=='FileUpload'">
							</FileUpload>
							<Location :ref="'form'+column.id" :is="column.name" :mode="mode" v-model="row[column.id]"
								v-bind="column.props" :keyId="column.id" v-if="column&&column.name=='Location'">
							</Location>
							<DeptPicker :ref="'form'+column.id" :is="column.name" :mode="mode" v-model="row[column.id]"
								v-bind="column.props" :keyId="column.id" @setActiveItem="setActiveItem"
								v-if="column&&column.name=='DeptPicker'"></DeptPicker>
							<UserPicker :ref="'form'+column.id" :is="column.name" :mode="mode" v-model="row[column.id]"
								v-bind="column.props" :keyId="column.id" @setActiveItem="setActiveItem"
								v-if="column&&column.name=='UserPicker'"></UserPicker>
							<DevicPicker :ref="'form'+column.id" :is="column.name" :mode="mode" v-model="row[column.id]"
								v-bind="column.props" :keyId="column.id" @setActiveItem="setActiveItem"
								v-if="column&&column.name=='DevicPicker'"></DevicPicker>
							<SignPanel :ref="'form'+column.id" :is="column.name" :mode="mode" v-model="row[column.id]"
								v-bind="column.props" :keyId="column.id" @setActiveItem="setActiveItem"
								v-if="column&&column.name=='SignPanel'"></SignPanel>
							<SpanLayout :ref="'form'+column.id" :is="column.name" :mode="mode" v-model="_value"
								v-bind="column.props" :keyId="column.id" @setActiveItem="setActiveItem"
								v-if="column&&column.name=='SpanLayout'"></SpanLayout>
							<TableList :ref="'form'+column.id" :is="column.name" :mode="mode" v-model="row[column.id]"
								v-bind="column.props" :keyId="column.id" @setActiveItem="setActiveItem"
								v-if="column&&column.name=='TableList'"></TableList>
							<ParamInput :ref="'form'+column.id" :is="column.name" :mode="mode" v-model="row[column.id]"
								v-bind="column.props" :keyId="column.id" @setActiveItem="setActiveItem"
								v-if="column&&column.name=='ParamInput'"></ParamInput>
						</view>
					</uni-forms-item>
					<view class="sort_num">
						{{i + 1}}
					</view>
				</view>
			</uni-forms>
		</view>
		<view class="table_con" v-else-if="!disabled&&rowLayout">
			<!-- <uni-forms :ref="`table-form-${i}`" :modelValue="row" :rules="rules" labelWidth='80' label-position="top"
				v-for="(row, i) in _value" :key="i">
				
			</uni-forms> -->

			<view class="" v-for="(row, i) in _value">
				<view class="table_li">
					<uni-forms-item :required="column.props&&column.props.required" :label="column.title"
						:name="column.id" :id="'form_'+column.id+'_'+i+index" labelFont="32rpx" contentFont="32rpx"
						:requireOpacity="0.5" v-for="(column, index) in _columns" :key="'column_' + index"
						:isDis="column.props&&column.props.disabled" :isfirstTop="index==0">
						<view :id="'li_'+column.id+'_'+i+index" class="form_li">
							<!-- <FormDesignRender v-model="row[column.id]" :mode="mode" :config="column" :keyId="column.id">
							</FormDesignRender> -->
							<TextInput :ref="'form'+column.id" :is="column.name" :mode="mode" v-model="row[column.id]"
								v-bind="column.props" :keyId="column.id" v-if="column&&column.name=='TextInput'">
							</TextInput>
							<NumberInput :ref="'form'+column.id" :is="column.name" :mode="mode" v-model="row[column.id]"
								v-bind="column.props" :keyId="column.id" v-if="column&&column.name=='NumberInput'">
							</NumberInput>
							<AmountInput :ref="'form'+column.id" :is="column.name" :mode="mode" v-model="row[column.id]"
								v-bind="column.props" :keyId="column.id" v-if="column&&column.name=='AmountInput'">
							</AmountInput>
							<TextareaInput :ref="'form'+column.id" :is="column.name" :mode="mode"
								v-model="row[column.id]" v-bind="column.props" :keyId="column.id"
								v-if="column&&column.name=='TextareaInput'"></TextareaInput>
							<SelectInput :ref="'form'+column.id" :is="column.name" :mode="mode" v-model="row[column.id]"
								v-bind="column.props" :keyId="column.id" v-if="column&&column.name=='SelectInput'">
							</SelectInput>
							<MultipleSelect :ref="'form'+column.id" :is="column.name" :mode="mode"
								v-model="row[column.id]" v-bind="column.props" :keyId="column.id"
								v-if="column&&column.name=='MultipleSelect'"></MultipleSelect>
							<DateTime :ref="'form'+column.id" :is="column.name" :mode="mode" v-model="row[column.id]"
								v-bind="column.props" :keyId="column.id" v-if="column&&column.name=='DateTime'">
							</DateTime>
							<DateTimeRange :ref="'form'+column.id" :is="column.name" :mode="mode"
								v-model="row[column.id]" v-bind="column.props" :keyId="column.id"
								v-if="column&&column.name=='DateTimeRange'"></DateTimeRange>
							<Description :ref="'form'+column.id" :is="column.name" :mode="mode" v-model="row[column.id]"
								v-bind="column.props" :keyId="column.id" v-if="column&&column.name=='Description'">
							</Description>
							<ImageUpload :ref="'form'+column.id" :is="column.name" :mode="mode" v-model="row[column.id]"
								v-bind="column.props" :keyId="column.id" v-if="column&&column.name=='ImageUpload'">
							</ImageUpload>
							<FileUpload :ref="'form'+column.id" :is="column.name" :mode="mode" v-model="row[column.id]"
								v-bind="column.props" :keyId="column.id" v-if="column&&column.name=='FileUpload'">
							</FileUpload>
							<Location :ref="'form'+column.id" :is="column.name" :mode="mode" v-model="row[column.id]"
								v-bind="column.props" :keyId="column.id" v-if="column&&column.name=='Location'">
							</Location>
							<DeptPicker :ref="'form'+column.id" :is="column.name" :mode="mode" v-model="row[column.id]"
								v-bind="column.props" :keyId="column.id" @setActiveItem="setActiveItem"
								v-if="column&&column.name=='DeptPicker'"></DeptPicker>
							<UserPicker :ref="'form'+column.id" :is="column.name" :mode="mode" v-model="row[column.id]"
								v-bind="column.props" :keyId="column.id" @setActiveItem="setActiveItem"
								v-if="column&&column.name=='UserPicker'"></UserPicker>
							<DevicPicker :ref="'form'+column.id" :is="column.name" :mode="mode" v-model="row[column.id]"
								v-bind="column.props" :keyId="column.id" @setActiveItem="setActiveItem"
								v-if="column&&column.name=='DevicPicker'"></DevicPicker>
							<SignPanel :ref="'form'+column.id" :is="column.name" :mode="mode" v-model="row[column.id]"
								v-bind="column.props" :keyId="column.id" @setActiveItem="setActiveItem"
								v-if="column&&column.name=='SignPanel'"></SignPanel>
							<SpanLayout :ref="'form'+column.id" :is="column.name" :mode="mode" v-model="_value"
								v-bind="column.props" :keyId="column.id" @setActiveItem="setActiveItem"
								v-if="column&&column.name=='SpanLayout'"></SpanLayout>
							<TableList :ref="'form'+column.id" :is="column.name" :mode="mode" v-model="row[column.id]"
								v-bind="column.props" :keyId="column.id" @setActiveItem="setActiveItem"
								v-if="column&&column.name=='TableList'"></TableList>
							<ParamInput :ref="'form'+column.id" :is="column.name" :mode="mode" v-model="row[column.id]"
								v-bind="column.props" :keyId="column.id" @setActiveItem="setActiveItem"
								v-if="column&&column.name=='ParamInput'"></ParamInput>

						</view>
					</uni-forms-item>
					<view class="table_btn">
						<view class="btn_li" @click="copyData(i, row)">Copy</view>
						<view class="btn_line"></view>
						<view class="btn_li" @click="delRow(i, row)">Delete</view>
					</view>
					<view class="sort_num">
						{{i + 1}}
					</view>
				</view>
			</view>
			<view class="form_device_add" @click.stop="addRow">
				<custom-icons iconsName="icon-tianjia" iconsSize="24rpx"
					iconsColor="rgba(255, 255, 255, 0.5)"></custom-icons>
				<view class="text">
					Add data
				</view>
			</view>
		</view>
		<view class="table_con" v-else-if="!disabled&&!rowLayout">
			<uni-forms :ref="`table-form-${i}`" :modelValue="row" :rules="rules" labelWidth='80' label-position="top"
				v-for="(row, i) in _value" :key="i">
				<view class="table_li">
					<uni-forms-item :required="column.props&&column.props.required" :label="column.title"
						:name="column.id" :id="'form_'+column.id+'_'+i+index" labelFont="32rpx" contentFont="32rpx"
						:requireOpacity="0.5" v-for="(column, index) in _columns" :key="'column_' + index"
						:isDis="column.props&&column.props.disabled" :isfirstTop="index==0">
						<view :id="'li_'+column.id+'_'+i+index" class="form_li">
							<TextInput :ref="'form'+column.id" :is="column.name" :mode="mode" v-model="row[column.id]"
								v-bind="column.props" :keyId="column.id" v-if="column&&column.name=='TextInput'">
							</TextInput>
							<NumberInput :ref="'form'+column.id" :is="column.name" :mode="mode" v-model="row[column.id]"
								v-bind="column.props" :keyId="column.id" v-if="column&&column.name=='NumberInput'">
							</NumberInput>
							<AmountInput :ref="'form'+column.id" :is="column.name" :mode="mode" v-model="row[column.id]"
								v-bind="column.props" :keyId="column.id" v-if="column&&column.name=='AmountInput'">
							</AmountInput>
							<TextareaInput :ref="'form'+column.id" :is="column.name" :mode="mode"
								v-model="row[column.id]" v-bind="column.props" :keyId="column.id"
								v-if="column&&column.name=='TextareaInput'"></TextareaInput>
							<SelectInput :ref="'form'+column.id" :is="column.name" :mode="mode" v-model="row[column.id]"
								v-bind="column.props" :keyId="column.id" v-if="column&&column.name=='SelectInput'">
							</SelectInput>
							<MultipleSelect :ref="'form'+column.id" :is="column.name" :mode="mode"
								v-model="row[column.id]" v-bind="column.props" :keyId="column.id"
								v-if="column&&column.name=='MultipleSelect'"></MultipleSelect>
							<DateTime :ref="'form'+column.id" :is="column.name" :mode="mode" v-model="row[column.id]"
								v-bind="column.props" :keyId="column.id" v-if="column&&column.name=='DateTime'">
							</DateTime>
							<DateTimeRange :ref="'form'+column.id" :is="column.name" :mode="mode"
								v-model="row[column.id]" v-bind="column.props" :keyId="column.id"
								v-if="column&&column.name=='DateTimeRange'"></DateTimeRange>
							<Description :ref="'form'+column.id" :is="column.name" :mode="mode" v-model="row[column.id]"
								v-bind="column.props" :keyId="column.id" v-if="column&&column.name=='Description'">
							</Description>
							<ImageUpload :ref="'form'+column.id" :is="column.name" :mode="mode" v-model="row[column.id]"
								v-bind="column.props" :keyId="column.id" v-if="column&&column.name=='ImageUpload'">
							</ImageUpload>
							<FileUpload :ref="'form'+column.id" :is="column.name" :mode="mode" v-model="row[column.id]"
								v-bind="column.props" :keyId="column.id" v-if="column&&column.name=='FileUpload'">
							</FileUpload>
							<Location :ref="'form'+column.id" :is="column.name" :mode="mode" v-model="row[column.id]"
								v-bind="column.props" :keyId="column.id" v-if="column&&column.name=='Location'">
							</Location>
							<DeptPicker :ref="'form'+column.id" :is="column.name" :mode="mode" v-model="row[column.id]"
								v-bind="column.props" :keyId="column.id" @setActiveItem="setActiveItem"
								v-if="column&&column.name=='DeptPicker'"></DeptPicker>
							<UserPicker :ref="'form'+column.id" :is="column.name" :mode="mode" v-model="row[column.id]"
								v-bind="column.props" :keyId="column.id" @setActiveItem="setActiveItem"
								v-if="column&&column.name=='UserPicker'"></UserPicker>
							<DevicPicker :ref="'form'+column.id" :is="column.name" :mode="mode" v-model="row[column.id]"
								v-bind="column.props" :keyId="column.id" @setActiveItem="setActiveItem"
								v-if="column&&column.name=='DevicPicker'"></DevicPicker>
							<SignPanel :ref="'form'+column.id" :is="column.name" :mode="mode" v-model="row[column.id]"
								v-bind="column.props" :keyId="column.id" @setActiveItem="setActiveItem"
								v-if="column&&column.name=='SignPanel'"></SignPanel>
							<SpanLayout :ref="'form'+column.id" :is="column.name" :mode="mode" v-model="_value"
								v-bind="column.props" :keyId="column.id" @setActiveItem="setActiveItem"
								v-if="column&&column.name=='SpanLayout'"></SpanLayout>
							<TableList :ref="'form'+column.id" :is="column.name" :mode="mode" v-model="row[column.id]"
								v-bind="column.props" :keyId="column.id" @setActiveItem="setActiveItem"
								v-if="column&&column.name=='TableList'"></TableList>
							<ParamInput :ref="'form'+column.id" :is="column.name" :mode="mode" v-model="row[column.id]"
								v-bind="column.props" :keyId="column.id" @setActiveItem="setActiveItem"
								v-if="column&&column.name=='ParamInput'"></ParamInput>
						</view>
					</uni-forms-item>
					<view class="table_btn">
						<view class="btn_li" @click="copyData(i, row)">Copy</view>
						<view class="btn_line"></view>
						<view class="btn_li" @click="delRow(i, row)">Delete</view>
					</view>
					<view class="sort_num">
						{{i + 1}}
					</view>
				</view>
			</uni-forms>
			<view class="form_device_add" @click.stop="addRow">
				<custom-icons iconsName="icon-tianjia" iconsSize="24rpx"
					iconsColor="rgba(255, 255, 255, 0.5)"></custom-icons>
				<view class="text">
					Add data
				</view>
			</view>
		</view>
		<view class="dis_text" v-if="showSummary" style="margin-top: 20rpx;">
			合计：{{totalval}}{{summaryUnit}}
		</view>
		<msg-prompt ref="promptMsg"></msg-prompt>
		<!-- </view> -->
	</view>
</template>

<script>
	import {
		ValueType
	} from "../ComponentsConfigExport";
	import FormDesignRender from "@/components/flow-form/form-design-render2.vue";
	import TextInput from './text-input.vue'
	import NumberInput from './number-input.vue'
	import AmountInput from './amount-input.vue'
	import TextareaInput from './textarea-input.vue'
	import SelectInput from './select-input.vue'
	import MultipleSelect from './multiple-select.vue'
	import DateTime from './DateTime.vue'
	import DateTimeRange from './DateTimeRange.vue'
	import Description from './Description.vue'
	import ImageUpload from './ImageUpload.vue'
	import FileUpload from './FileUpload.vue'
	import Location from './Location.vue'
	import DeptPicker from './DeptPicker.vue'
	import UserPicker from './UserPicker.vue'
	import DevicPicker from './DevicPicker.vue'
	import SignPanel from './SignPannel.vue'
	import SpanLayout from './SpanLayout.vue'
	import TableList from './TableList.vue'
	import ParamInput from './ParamInput.vue'
	import componentMinxins from "../ComponentMinxins";
	import {
		deepCopy
	} from "@/pages_flow/utlity.js";

	export default {
		mixins: [componentMinxins],
		name: "TableList",
		components: {
			FormDesignRender,
			TextInput,
			NumberInput,
			AmountInput,
			TextareaInput,
			SelectInput,
			MultipleSelect,
			DateTime,
			DateTimeRange,
			Description,
			ImageUpload,
			FileUpload,
			Location,
			DeptPicker,
			UserPicker,
			DevicPicker,
			SignPanel,
			SpanLayout,
			TableList,
			ParamInput
		},
		props: {
			value: {
				type: Array,
				default: () => {
					return [];
				},
			},
			placeholder: {
				type: String,
				default: "添加数据",
			},
			columns: {
				type: Array,
				default: () => {
					return [];
				},
			},
			showBorder: {
				type: Boolean,
				default: true,
			},
			maxSize: {
				type: Number,
				default: 0,
			},
			rowLayout: {
				type: Boolean,
				default: true,
			},
			disabled: {
				default: false,
				type: Boolean,
			},
			showSummary: {
				default: false,
				type: Boolean,
			},
			summaryColumns: {
				type: Array,
				default: () => {
					return [];
				},
			},
			summaryUnit: {
				type: String,
				default: "",
			},
		},
		// beforeCreate: function () {
		//   this.$options.components.SpanLayout = require('./SpanLayout.vue').default
		// },
		beforeUpdate() {
			if (!Array.isArray(this.value)) {
				this._value = [];
			}
			if (this.disabled) {
				this.columns.forEach((col) => {
					col.props.disabled = true;
				});
			}

		},
		beforeMount() {
			console.log(this._value, '_value_value_value');
			console.log("_columns", this._columns);
			if (!Array.isArray(this.value)) {
				this._value = [];
			}
			if (this.disabled) {
				this.columns.forEach((col) => {
					col.props.disabled = true;
				});
			}
			this.$forceUpdate()
		},
		computed: {
			totalval() {
				let totalnum = 0;
				if (this._value == null) {
					this._value = []
					this.$nextTick(() => {
						this.value.forEach(x => {
							let rowval = 1;
							this.summaryColumns.forEach(z => {
								rowval = rowval * x[z];
							})
							totalnum = totalnum + rowval;
						});
						return totalnum;
					})
				} else {
					this.value.forEach(x => {
						let rowval = 1;
						this.summaryColumns.forEach(z => {
							rowval = rowval * x[z];
						})
						totalnum = totalnum + rowval;
					});
					return totalnum;
				}


			},
			rules() {
				const rules = {};
				this.columns.map((col) => {
					if (col.props.required) {
						rules[col.id] = {
							rules: [{
								type: col.valueType === "Array" ? "array" : undefined,
								required: true,
								errorMessage: `请填写${col.title}`,
							}]
						};

					}
				});
				return rules;
			},
			_columns: {
				get() {
					return this.columns;
				},
				set(val) {
					this.columns = val;
				},
			},
			selectFormItem: {
				get() {
					return this.$store.state.flowable.selectFormItem;
				},
				set(val) {
					this.$store.state.flowable.selectFormItem = val;
				},
			},
		},
		data() {
			return {
				select: null,
				drag: false,
				ValueType,
			};
		},
		methods: {
			setActiveItem(val){
				this.$emit('setActiveItem',val)
			},
			selectDept(val,acitveId){
				//选择部门
				this.$refs[`form${acitveId}`].selectDept(val,acitveId)
			},
			selectEmplee(val,acitveId){
				//选择人员
				this.$refs[`form${acitveId}`].selectEmplee(val,acitveId)
			},
			selectDevice(val,acitveId){
				//选择设备
				this.$refs[`form${acitveId}`].selectDevice(val,acitveId)
			},
			getMinWidth(col) {
				switch (col.name) {
					case "DateTime":
						return "250px";
					case "DateTimeRange":
						return "280px";
					case "MultipleSelect":
						return "200px";
					default:
						return "150px";
				}
			},
			showError(col, val) {
				if (col.props.required) {
					switch (col.valueType) {
						case ValueType.dept:
						case ValueType.user:
						case ValueType.dateRange:
						case ValueType.array:
							return !(Array.isArray(val) && val.length > 0);
						default:
							console.log('空字符验证', val)
							return !this.$isNotEmpty(val);
					}
				}
				return false;
			},
			copyData(i, row) {
				this._value.push(deepCopy(row));
			},
			delRow(i, row) {
				this._value.splice(i, 1);
			},
			addRow() {
				if (this.maxSize > 0 && this._value.length >= this.maxSize) {
					// this.$message.warning(`最多只能添加${this.maxSize}行`);
					uni.showToast({
						title: `最多只能添加${this.maxSize}行`
					})
				} else {
					let row = {};
					this.columns.forEach((col) => this.$set(row, col.id, undefined));
					// console.info(this._value);
					// console.info(row);
					if (this._value == null) {
						this.$set(this, "_value", [row]);
					} else {
						this._value.push(row);
						this.$set(this, "_value", this._value);
					}
				}

			},
			delItem(id) {
				this._columns.splice(id, 1);
			},
			selectItem(cp) {
				this.selectFormItem = cp;
			},
			getSelectedClass(cp) {
				return this.selectFormItem && this.selectFormItem.id === cp.id ?
					"border-left: 4px solid #f56c6c" :
					"";
			},
			async validate(call) {
				if (this.rowLayout) {
					let result = true;
					for (let i = 0; i < this.columns.length; i++) {
						if (this.columns[i].props.required) {
							for (let j = 0; j < this._value.length; j++) {
								result = !this.showError(
									this.columns[i],
									this._value[j][this.columns[i].id]
								);
								if (!result) {
									// call(false);
									console.log(this.columns[i], '表格内', this.columns[i].id);

									let firstErr = this.columns[i].id + '_' + j + i
									// #ifdef MP-WEIXIN
									const query = uni.createSelectorQuery().in(this);
									let str = '#li_' + firstErr
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
										selector: '#form_' + firstErr,
										duration: 300
									});
									// #endif 
									this.$refs.promptMsg.open('please enter ' + this.columns[i].title, 2000)
									return false;
								}
							}
						}
					}
					// call(result);
					return result;
				} else {
					let success = 0;
					for (let i = 0; i < this.value.length; i++) {
						let formRef = this.$refs[`table-form-${i}`];
						if (formRef && Array.isArray(formRef) && formRef.length > 0) {
							try{
								let valid=await formRef[0].validate()
								if (valid) {
									success++;
								}
							}catch(err){
								//TODO handle the exception
								console.log("err", err);
								if (err && err.length > 0) {
									let firstErr = err[0].key
									// #ifdef MP-WEIXIN
									const query = uni.createSelectorQuery().in(this);
									let str = '#li_' + firstErr
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
										selector: '#form_' + firstErr,
										duration: 300
									});
									// #endif  
								}
							}
							// formRef[0].validate().then(valid => {
							// 	console.log("valid",valid);
							// 	if (valid) {
							// 		success++;
							// 	}
							// }).catch(err => {
								
							// });
						}
					}
					// this._value.map((v, i) => {

					// });
					console.log(success,'successsuccesssuccess');
					if (success === this._value.length) {
						// call(true);
						return true
					} else {
						// call(false);
						return false;
					}
				}
			},
		},
	};
</script>

<style lang="less" scoped>

</style>