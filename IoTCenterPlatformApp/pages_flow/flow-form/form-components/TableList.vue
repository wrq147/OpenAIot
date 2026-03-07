<template>
	<view style="width: 100%;">
		<view class="table_con" v-if="disabled&&rowLayout" :class="{'disable_table':disabled}">
			<view class="" v-for="(row, i) in _value">
				<view class="table_li">
					<!-- :id="'form_'+column.id+'_'+i+index" -->
					<uni-forms-item :required="column.props&&column.props.required" :label="column.title"
						:name="column.id" labelFont="28rpx" contentFont="32rpx" :requireOpacity="0.5"
						v-for="(column, index) in _columns" :isDis="column.props&&column.props.disabled"
						:isfirstTop="disabled||index==0">
						<view :id="'li_'+column.id+'_'+i+index" class="form_li">
							<TextInput @update:value="updateval($event,i,column.id)" style="width: 100%;"
								:ref="'form'+column.id" :mode="mode" :value="row[column.id]"
								:enablePrint="column.props.enablePrint" :defaultValue="column.props.defaultValue"
								:required="column.props.required" :keyId="column.id"
								v-if="column&&column.name=='TextInput'">
							</TextInput>
							<NumberInput @update:value="updateval($event,i,column.id)" style="width: 100%;"
								:ref="'form'+column.id" :mode="mode" :value="row[column.id]" :min="column.props.min"
								:max="column.props.max" :precision="column.props.precision"
								:enablePrint="column.props.enablePrint" :required="column.props.required"
								:keyId="column.id" v-if="column&&column.name=='NumberInput'">
							</NumberInput>
							<AmountInput @update:value="updateval($event,i,column.id)" style="width: 100%;"
								:ref="'form'+column.id" :mode="mode" :value="row[column.id]"
								:showChinese="column.props.showChinese" :enablePrint="column.props.enablePrint"
								:required="column.props.required" :keyId="column.id"
								v-if="column&&column.name=='AmountInput'">
							</AmountInput>
							<TextareaInput @update:value="updateval($event,i,column.id)" style="width: 100%;"
								:ref="'form'+column.id" :mode="mode" :value="row[column.id]"
								:enablePrint="column.props.enablePrint" :required="column.props.required"
								:keyId="column.id" v-if="column&&column.name=='TextareaInput'"></TextareaInput>
							<SelectInput @update:value="updateval($event,i,column.id)" style="width: 100%;"
								:ref="'form'+column.id" :mode="mode" :value="row[column.id]"
								:options="column.props.options" :expanding="column.props.expanding"
								:enablePrint="column.props.enablePrint" :required="column.props.required"
								:keyId="column.id" v-if="column&&column.name=='SelectInput'">
							</SelectInput>
							<MultipleSelect @update:value="updateval($event,i,column.id)" style="width: 100%;"
								:ref="'form'+column.id" :mode="mode" :value="row[column.id]"
								:options="column.props.options" :expanding="column.props.expanding"
								:enablePrint="column.props.enablePrint" :required="column.props.required"
								:keyId="column.id" v-if="column&&column.name=='MultipleSelect'"></MultipleSelect>
							<DateTime @update:value="updateval($event,i,column.id)" style="width: 100%;"
								:ref="'form'+column.id" :mode="mode" :value="row[column.id]"
								:format="column.props.format" :enablePrint="column.props.enablePrint"
								:required="column.props.required" :keyId="column.id"
								v-if="column&&column.name=='DateTime'">
							</DateTime>
							<DateTimeRange @update:value="updateval($event,i,column.id)" style="width: 100%;"
								:ref="'form'+column.id" :mode="mode" :value="row[column.id]"
								:showLength="column.props.showLength" :placeholder="column.props.placeholder"
								:format="column.props.format" :enablePrint="column.props.enablePrint"
								:required="column.props.required" :keyId="column.id"
								v-if="column&&column.name=='DateTimeRange'"></DateTimeRange>
							<Description @update:value="updateval($event,i,column.id)" style="width: 100%;"
								:ref="'form'+column.id" :mode="mode" :value="row[column.id]"
								:enablePrint="column.props.enablePrint" :required="column.props.required"
								:keyId="column.id" v-if="column&&column.name=='Description'">
							</Description>
							<ImageUpload @update:value="updateval($event,i,column.id)" style="width: 100%;"
								:ref="'form'+column.id" :mode="mode" :value="row[column.id]"
								:enableZip="column.props.enableZip" :maxNumber="column.props.maxNumber"
								:maxSize="column.props.maxSize" :enablePrint="column.props.enablePrint"
								:required="column.props.required" :keyId="column.id"
								v-if="column&&column.name=='ImageUpload'">
							</ImageUpload>
							<FileUpload @update:value="updateval($event,i,column.id)" style="width: 100%;"
								:ref="'form'+column.id" :mode="mode" :value="row[column.id]"
								:fileTypes="column.props.fileTypes" :onlyRead="column.props.onlyRead"
								:maxNumber="column.props.maxNumber" :maxSize="column.props.maxSize"
								:enablePrint="column.props.enablePrint" :required="column.props.required"
								:keyId="column.id" v-if="column&&column.name=='FileUpload'">
							</FileUpload>
							<Location @update:value="updateval($event,i,column.id)" style="width: 100%;"
								:ref="'form'+column.id" :mode="mode" :value="row[column.id]" :keyId="column.id"
								v-if="column&&column.name=='Location'">
							</Location>
							<DeptPicker @update:value="updateval($event,i,column.id)" style="width: 100%;"
								:ref="'form'+column.id" :mode="mode" :value="row[column.id]"
								:multiple="column.props.multiple" :enablePrint="column.props.enablePrint"
								:required="column.props.required" :keyId="column.id" @setActiveItem="setActiveItem"
								v-if="column&&column.name=='DeptPicker'"></DeptPicker>
							<UserPicker @update:value="updateval($event,i,column.id)" style="width: 100%;"
								:ref="'form'+column.id" :mode="mode" :value="row[column.id]"
								:multiple="column.props.multiple" :enablePrint="column.props.enablePrint"
								:required="column.props.required" :keyId="column.id" @setActiveItem="setActiveItem"
								v-if="column&&column.name=='UserPicker'"></UserPicker>
							<DevicPicker @update:value="updateval($event,i,column.id)" style="width: 100%;"
								:ref="'form'+column.id" :mode="mode" :value="row[column.id]"
								:synclist="column.props.synclist" :limit_product="column.props.limit_product"
								:limit="column.props.limit" :enablePrint="column.props.enablePrint"
								:required="column.props.required" :keyId="column.id" @setActiveItem="setActiveItem"
								v-if="column&&column.name=='DevicPicker'"></DevicPicker>
							<SignPanel @update:value="updateval($event,i,column.id)" style="width: 100%;"
								:ref="'form'+column.id" :mode="mode" :value="row[column.id]" :keyId="column.id"
								@setActiveItem="setActiveItem" v-if="column&&column.name=='SignPanel'"></SignPanel>
							<SpanLayout @update:value="updateval($event,i,column.id)" style="width: 100%;"
								:ref="'form'+column.id" :mode="mode" :value="_value" :items="column.props.items"
								:keyId="column.id" @setActiveItem="setActiveItem"
								v-if="column&&column.name=='SpanLayout'"></SpanLayout>
							<TableList @update:value="updateval($event,i,column.id)" style="width: 100%;"
								:ref="'form'+column.id" :mode="mode" :value="row[column.id]"
								:columns="column.props.columns" :maxSize="column.props.maxSize"
								:summaryUnit="column.props.summaryUnit" :summaryColumns="column.props.summaryColumns"
								:deductid="column.props.deductid" :showSummary="column.props.showSummary"
								:IdxColName="column.props.IdxColName" :rowLayout="column.props.rowLayout"
								:showBorder="column.props.showBorder" :enablePrint="column.props.enablePrint"
								:required="column.props.required" :keyId="column.id" @setActiveItem="setActiveItem"
								v-if="column&&column.name=='TableList'"></TableList>
							<ParamInput @update:value="updateval($event,i,column.id)" style="width: 100%;"
								:ref="'form'+column.id" :mode="mode" :value="row[column.id]"
								:formId="column.props.formId" :formType="column.props.formType"
								:enablePrint="column.props.enablePrint" :required="column.props.required"
								:keyId="column.id" @setActiveItem="setActiveItem"
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
		<view class="table_con" v-else-if="disabled&&!rowLayout" :class="{'disable_table':disabled}">
			<uni-forms :ref="`table-form-${i}`" :modelValue="row" :rules="rules" labelWidth='80' label-position="top"
				v-for="(row, i) in _value">
				<view class="table_li">
					<!-- :id="'form_'+column.id+'_'+i+index" -->
					<uni-forms-item :required="column.props&&column.props.required" :label="column.title"
						:name="column.id" labelFont="28rpx" contentFont="32rpx" :requireOpacity="0.5"
						v-for="(column, index) in _columns" :isDis="column.props&&column.props.disabled"
						:isfirstTop="disabled||index==0">
						<view :id="'li_'+column.id+'_'+i+index" class="form_li">
							<TextInput @update:value="updateval($event,i,column.id)" style="width: 100%;"
								:ref="'form'+column.id" :mode="mode" :value="row[column.id]"
								:enablePrint="column.props.enablePrint" :defaultValue="column.props.defaultValue"
								:required="column.props.required" :keyId="column.id"
								v-if="column&&column.name=='TextInput'">
							</TextInput>
							<NumberInput @update:value="updateval($event,i,column.id)" style="width: 100%;"
								:ref="'form'+column.id" :mode="mode" :value="row[column.id]" :min="column.props.min"
								:max="column.props.max" :precision="column.props.precision"
								:enablePrint="column.props.enablePrint" :required="column.props.required"
								:keyId="column.id" v-if="column&&column.name=='NumberInput'">
							</NumberInput>
							<AmountInput @update:value="updateval($event,i,column.id)" style="width: 100%;"
								:ref="'form'+column.id" :mode="mode" :value="row[column.id]"
								:showChinese="column.props.showChinese" :enablePrint="column.props.enablePrint"
								:required="column.props.required" :keyId="column.id"
								v-if="column&&column.name=='AmountInput'">
							</AmountInput>
							<TextareaInput @update:value="updateval($event,i,column.id)" style="width: 100%;"
								:ref="'form'+column.id" :mode="mode" :value="row[column.id]"
								:enablePrint="column.props.enablePrint" :required="column.props.required"
								:keyId="column.id" v-if="column&&column.name=='TextareaInput'"></TextareaInput>
							<SelectInput @update:value="updateval($event,i,column.id)" style="width: 100%;"
								:ref="'form'+column.id" :mode="mode" :value="row[column.id]"
								:options="column.props.options" :expanding="column.props.expanding"
								:enablePrint="column.props.enablePrint" :required="column.props.required"
								:keyId="column.id" v-if="column&&column.name=='SelectInput'">
							</SelectInput>
							<MultipleSelect @update:value="updateval($event,i,column.id)" style="width: 100%;"
								:ref="'form'+column.id" :mode="mode" :value="row[column.id]"
								:options="column.props.options" :expanding="column.props.expanding"
								:enablePrint="column.props.enablePrint" :required="column.props.required"
								:keyId="column.id" v-if="column&&column.name=='MultipleSelect'"></MultipleSelect>
							<DateTime @update:value="updateval($event,i,column.id)" style="width: 100%;"
								:ref="'form'+column.id" :mode="mode" :value="row[column.id]"
								:format="column.props.format" :enablePrint="column.props.enablePrint"
								:required="column.props.required" :keyId="column.id"
								v-if="column&&column.name=='DateTime'">
							</DateTime>
							<DateTimeRange @update:value="updateval($event,i,column.id)" style="width: 100%;"
								:ref="'form'+column.id" :mode="mode" :value="row[column.id]"
								:showLength="column.props.showLength" :placeholder="column.props.placeholder"
								:format="column.props.format" :enablePrint="column.props.enablePrint"
								:required="column.props.required" :keyId="column.id"
								v-if="column&&column.name=='DateTimeRange'"></DateTimeRange>
							<Description @update:value="updateval($event,i,column.id)" style="width: 100%;"
								:ref="'form'+column.id" :mode="mode" :value="row[column.id]"
								:enablePrint="column.props.enablePrint" :required="column.props.required"
								:keyId="column.id" v-if="column&&column.name=='Description'">
							</Description>
							<ImageUpload @update:value="updateval($event,i,column.id)" style="width: 100%;"
								:ref="'form'+column.id" :mode="mode" :value="row[column.id]"
								:enableZip="column.props.enableZip" :maxNumber="column.props.maxNumber"
								:maxSize="column.props.maxSize" :enablePrint="column.props.enablePrint"
								:required="column.props.required" :keyId="column.id"
								v-if="column&&column.name=='ImageUpload'">
							</ImageUpload>
							<FileUpload @update:value="updateval($event,i,column.id)" style="width: 100%;"
								:ref="'form'+column.id" :mode="mode" :value="row[column.id]"
								:fileTypes="column.props.fileTypes" :onlyRead="column.props.onlyRead"
								:maxNumber="column.props.maxNumber" :maxSize="column.props.maxSize"
								:enablePrint="column.props.enablePrint" :required="column.props.required"
								:keyId="column.id" v-if="column&&column.name=='FileUpload'">
							</FileUpload>
							<Location @update:value="updateval($event,i,column.id)" style="width: 100%;"
								:ref="'form'+column.id" :mode="mode" :value="row[column.id]" :keyId="column.id"
								v-if="column&&column.name=='Location'">
							</Location>
							<DeptPicker @update:value="updateval($event,i,column.id)" style="width: 100%;"
								:ref="'form'+column.id" :mode="mode" :value="row[column.id]"
								:multiple="column.props.multiple" :enablePrint="column.props.enablePrint"
								:required="column.props.required" :keyId="column.id" @setActiveItem="setActiveItem"
								v-if="column&&column.name=='DeptPicker'"></DeptPicker>
							<UserPicker @update:value="updateval($event,i,column.id)" style="width: 100%;"
								:ref="'form'+column.id" :mode="mode" :value="row[column.id]"
								:multiple="column.props.multiple" :enablePrint="column.props.enablePrint"
								:required="column.props.required" :keyId="column.id" @setActiveItem="setActiveItem"
								v-if="column&&column.name=='UserPicker'"></UserPicker>
							<DevicPicker @update:value="updateval($event,i,column.id)" style="width: 100%;"
								:ref="'form'+column.id" :mode="mode" :value="row[column.id]"
								:synclist="column.props.synclist" :limit_product="column.props.limit_product"
								:limit="column.props.limit" :enablePrint="column.props.enablePrint"
								:required="column.props.required" :keyId="column.id" @setActiveItem="setActiveItem"
								v-if="column&&column.name=='DevicPicker'"></DevicPicker>
							<SignPanel @update:value="updateval($event,i,column.id)" style="width: 100%;"
								:ref="'form'+column.id" :mode="mode" :value="row[column.id]" :keyId="column.id"
								@setActiveItem="setActiveItem" v-if="column&&column.name=='SignPanel'"></SignPanel>
							<SpanLayout @update:value="updateval($event,i,column.id)" style="width: 100%;"
								:ref="'form'+column.id" :mode="mode" :value="_value" :items="column.props.items"
								:keyId="column.id" @setActiveItem="setActiveItem"
								v-if="column&&column.name=='SpanLayout'"></SpanLayout>
							<TableList @update:value="updateval($event,i,column.id)" style="width: 100%;"
								:ref="'form'+column.id" :mode="mode" :value="row[column.id]"
								:columns="column.props.columns" :maxSize="column.props.maxSize"
								:summaryUnit="column.props.summaryUnit" :summaryColumns="column.props.summaryColumns"
								:deductid="column.props.deductid" :showSummary="column.props.showSummary"
								:IdxColName="column.props.IdxColName" :rowLayout="column.props.rowLayout"
								:showBorder="column.props.showBorder" :enablePrint="column.props.enablePrint"
								:required="column.props.required" :keyId="column.id" @setActiveItem="setActiveItem"
								v-if="column&&column.name=='TableList'"></TableList>
							<ParamInput @update:value="updateval($event,i,column.id)" style="width: 100%;"
								:ref="'form'+column.id" :mode="mode" :value="row[column.id]"
								:formId="column.props.formId" :formType="column.props.formType"
								:enablePrint="column.props.enablePrint" :required="column.props.required"
								:keyId="column.id" @setActiveItem="setActiveItem"
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
					<!-- :id="'form_'+column.id+'_'+i+index" -->
					<uni-forms-item :required="column.props&&column.props.required" :label="column.title"
						:name="column.id" labelFont="28rpx" contentFont="32rpx" :requireOpacity="0.5"
						v-for="(column, index) in _columns" :isDis="column.props&&column.props.disabled"
						:isfirstTop="index==0">
						<view :id="'li_'+column.id+'_'+i+index" class="form_li">
							<!-- <FormDesignRender :value="row[column.id]" :mode="mode" :config="column" :keyId="column.id">
							</FormDesignRender> -->
							<TextInput @update:value="updateval($event,i,column.id)" style="width: 100%;"
								:ref="'form'+column.id" :mode="mode" :value="row[column.id]"
								:enablePrint="column.props.enablePrint" :defaultValue="column.props.defaultValue"
								:required="column.props.required" :keyId="column.id"
								v-if="column&&column.name=='TextInput'">
							</TextInput>
							<NumberInput @update:value="updateval($event,i,column.id)" style="width: 100%;"
								:ref="'form'+column.id" :mode="mode" :value="row[column.id]" :min="column.props.min"
								:max="column.props.max" :precision="column.props.precision"
								:enablePrint="column.props.enablePrint" :required="column.props.required"
								:keyId="column.id" v-if="column&&column.name=='NumberInput'">
							</NumberInput>
							<AmountInput @update:value="updateval($event,i,column.id)" style="width: 100%;"
								:ref="'form'+column.id" :mode="mode" :value="row[column.id]"
								:showChinese="column.props.showChinese" :enablePrint="column.props.enablePrint"
								:required="column.props.required" :keyId="column.id"
								v-if="column&&column.name=='AmountInput'">
							</AmountInput>
							<TextareaInput @update:value="updateval($event,i,column.id)" style="width: 100%;"
								:ref="'form'+column.id" :mode="mode" :value="row[column.id]"
								:enablePrint="column.props.enablePrint" :required="column.props.required"
								:keyId="column.id" v-if="column&&column.name=='TextareaInput'"></TextareaInput>
							<SelectInput @update:value="updateval($event,i,column.id)" style="width: 100%;"
								:ref="'form'+column.id" :mode="mode" :value="row[column.id]"
								:options="column.props.options" :expanding="column.props.expanding"
								:enablePrint="column.props.enablePrint" :required="column.props.required"
								:keyId="column.id" v-if="column&&column.name=='SelectInput'">
							</SelectInput>
							<MultipleSelect @update:value="updateval($event,i,column.id)" style="width: 100%;"
								:ref="'form'+column.id" :mode="mode" :value="row[column.id]"
								:options="column.props.options" :expanding="column.props.expanding"
								:enablePrint="column.props.enablePrint" :required="column.props.required"
								:keyId="column.id" v-if="column&&column.name=='MultipleSelect'"></MultipleSelect>
							<DateTime @update:value="updateval($event,i,column.id)" style="width: 100%;"
								:ref="'form'+column.id" :mode="mode" :value="row[column.id]"
								:format="column.props.format" :enablePrint="column.props.enablePrint"
								:required="column.props.required" :keyId="column.id"
								v-if="column&&column.name=='DateTime'">
							</DateTime>
							<DateTimeRange @update:value="updateval($event,i,column.id)" style="width: 100%;"
								:ref="'form'+column.id" :mode="mode" :value="row[column.id]"
								:showLength="column.props.showLength" :placeholder="column.props.placeholder"
								:format="column.props.format" :enablePrint="column.props.enablePrint"
								:required="column.props.required" :keyId="column.id"
								v-if="column&&column.name=='DateTimeRange'"></DateTimeRange>
							<Description @update:value="updateval($event,i,column.id)" style="width: 100%;"
								:ref="'form'+column.id" :mode="mode" :value="row[column.id]"
								:enablePrint="column.props.enablePrint" :required="column.props.required"
								:keyId="column.id" v-if="column&&column.name=='Description'">
							</Description>
							<ImageUpload @update:value="updateval($event,i,column.id)" style="width: 100%;"
								:ref="'form'+column.id" :mode="mode" :value="row[column.id]"
								:enableZip="column.props.enableZip" :maxNumber="column.props.maxNumber"
								:maxSize="column.props.maxSize" :enablePrint="column.props.enablePrint"
								:required="column.props.required" :keyId="column.id"
								v-if="column&&column.name=='ImageUpload'">
							</ImageUpload>
							<FileUpload @update:value="updateval($event,i,column.id)" style="width: 100%;"
								:ref="'form'+column.id" :mode="mode" :value="row[column.id]"
								:fileTypes="column.props.fileTypes" :onlyRead="column.props.onlyRead"
								:maxNumber="column.props.maxNumber" :maxSize="column.props.maxSize"
								:enablePrint="column.props.enablePrint" :required="column.props.required"
								:keyId="column.id" v-if="column&&column.name=='FileUpload'">
							</FileUpload>
							<Location @update:value="updateval($event,i,column.id)" style="width: 100%;"
								:ref="'form'+column.id" :mode="mode" :value="row[column.id]" :keyId="column.id"
								v-if="column&&column.name=='Location'">
							</Location>
							<DeptPicker @update:value="updateval($event,i,column.id)" style="width: 100%;"
								:ref="'form'+column.id" :mode="mode" :value="row[column.id]"
								:multiple="column.props.multiple" :enablePrint="column.props.enablePrint"
								:required="column.props.required" :keyId="column.id" @setActiveItem="setActiveItem"
								v-if="column&&column.name=='DeptPicker'"></DeptPicker>
							<UserPicker @update:value="updateval($event,i,column.id)" style="width: 100%;"
								:ref="'form'+column.id" :mode="mode" :value="row[column.id]"
								:multiple="column.props.multiple" :enablePrint="column.props.enablePrint"
								:required="column.props.required" :keyId="column.id" @setActiveItem="setActiveItem"
								v-if="column&&column.name=='UserPicker'"></UserPicker>
							<DevicPicker @update:value="updateval($event,i,column.id)" style="width: 100%;"
								:ref="'form'+column.id" :mode="mode" :value="row[column.id]"
								:synclist="column.props.synclist" :limit_product="column.props.limit_product"
								:limit="column.props.limit" :enablePrint="column.props.enablePrint"
								:required="column.props.required" :keyId="column.id" @setActiveItem="setActiveItem"
								v-if="column&&column.name=='DevicPicker'"></DevicPicker>
							<SignPanel @update:value="updateval($event,i,column.id)" style="width: 100%;"
								:ref="'form'+column.id" :mode="mode" :value="row[column.id]" :keyId="column.id"
								@setActiveItem="setActiveItem" v-if="column&&column.name=='SignPanel'"></SignPanel>
							<SpanLayout @update:value="updateval($event,i,column.id)" style="width: 100%;"
								:ref="'form'+column.id" :mode="mode" :value="_value" :items="column.props.items"
								:keyId="column.id" @setActiveItem="setActiveItem"
								v-if="column&&column.name=='SpanLayout'"></SpanLayout>
							<TableList @update:value="updateval($event,i,column.id)" style="width: 100%;"
								:ref="'form'+column.id" :mode="mode" :value="row[column.id]"
								:columns="column.props.columns" :maxSize="column.props.maxSize"
								:summaryUnit="column.props.summaryUnit" :summaryColumns="column.props.summaryColumns"
								:deductid="column.props.deductid" :showSummary="column.props.showSummary"
								:IdxColName="column.props.IdxColName" :rowLayout="column.props.rowLayout"
								:showBorder="column.props.showBorder" :enablePrint="column.props.enablePrint"
								:required="column.props.required" :keyId="column.id" @setActiveItem="setActiveItem"
								v-if="column&&column.name=='TableList'"></TableList>
							<ParamInput @update:value="updateval($event,i,column.id)" style="width: 100%;"
								:ref="'form'+column.id" :mode="mode" :value="row[column.id]"
								:formId="column.props.formId" :formType="column.props.formType"
								:enablePrint="column.props.enablePrint" :required="column.props.required"
								:keyId="column.id" @setActiveItem="setActiveItem"
								v-if="column&&column.name=='ParamInput'"></ParamInput>

						</view>
					</uni-forms-item>
					<view class="table_btn">
						<view class="btn_li active" @click="copyData(i, row)">复制</view>
						<view class="btn_line"></view>
						<view class="btn_li" @click="delRow(i, row)">删除</view>
					</view>
					<view class="sort_num">
						{{i + 1}}
					</view>
				</view>
			</view>
			<view class="form_device_add" @click.stop="addRow">
				<custom-icons iconsName="icon-tianjia" iconsSize="24rpx" iconsColor="#333333"></custom-icons>
				<view class="text">
					添加数据
				</view>
			</view>
		</view>
		<view class="table_con" v-else-if="!disabled&&!rowLayout">
			<uni-forms :ref="`table-form-${i}`" :modelValue="row" :rules="rules" labelWidth='80' label-position="top"
				v-for="(row, i) in _value" :key="i">
				<view class="table_li">
					<!-- :id="'form_'+column.id+'_'+i+index" -->
					<uni-forms-item :required="column.props&&column.props.required" :label="column.title"
						:name="column.id" labelFont="28rpx" contentFont="32rpx" :requireOpacity="0.5"
						v-for="(column, index) in _columns" :isDis="column.props&&column.props.disabled"
						:isfirstTop="index==0">
						<view :id="'li_'+column.id+'_'+i+index" class="form_li">
							<TextInput @update:value="updateval($event,i,column.id)" style="width: 100%;"
								:ref="'form'+column.id" :mode="mode" :value="row[column.id]"
								:enablePrint="column.props.enablePrint" :defaultValue="column.props.defaultValue"
								:required="column.props.required" :keyId="column.id"
								v-if="column&&column.name=='TextInput'">
							</TextInput>
							<NumberInput @update:value="updateval($event,i,column.id)" style="width: 100%;"
								:ref="'form'+column.id" :mode="mode" :value="row[column.id]" :min="column.props.min"
								:max="column.props.max" :precision="column.props.precision"
								:enablePrint="column.props.enablePrint" :required="column.props.required"
								:keyId="column.id" v-if="column&&column.name=='NumberInput'">
							</NumberInput>
							<AmountInput @update:value="updateval($event,i,column.id)" style="width: 100%;"
								:ref="'form'+column.id" :mode="mode" :value="row[column.id]"
								:showChinese="column.props.showChinese" :enablePrint="column.props.enablePrint"
								:required="column.props.required" :keyId="column.id"
								v-if="column&&column.name=='AmountInput'">
							</AmountInput>
							<TextareaInput @update:value="updateval($event,i,column.id)" style="width: 100%;"
								:ref="'form'+column.id" :mode="mode" :value="row[column.id]"
								:enablePrint="column.props.enablePrint" :required="column.props.required"
								:keyId="column.id" v-if="column&&column.name=='TextareaInput'"></TextareaInput>
							<SelectInput @update:value="updateval($event,i,column.id)" style="width: 100%;"
								:ref="'form'+column.id" :mode="mode" :value="row[column.id]"
								:options="column.props.options" :expanding="column.props.expanding"
								:enablePrint="column.props.enablePrint" :required="column.props.required"
								:keyId="column.id" v-if="column&&column.name=='SelectInput'">
							</SelectInput>
							<MultipleSelect @update:value="updateval($event,i,column.id)" style="width: 100%;"
								:ref="'form'+column.id" :mode="mode" :value="row[column.id]"
								:options="column.props.options" :expanding="column.props.expanding"
								:enablePrint="column.props.enablePrint" :required="column.props.required"
								:keyId="column.id" v-if="column&&column.name=='MultipleSelect'"></MultipleSelect>
							<DateTime @update:value="updateval($event,i,column.id)" style="width: 100%;"
								:ref="'form'+column.id" :mode="mode" :value="row[column.id]"
								:format="column.props.format" :enablePrint="column.props.enablePrint"
								:required="column.props.required" :keyId="column.id"
								v-if="column&&column.name=='DateTime'">
							</DateTime>
							<DateTimeRange @update:value="updateval($event,i,column.id)" style="width: 100%;"
								:ref="'form'+column.id" :mode="mode" :value="row[column.id]"
								:showLength="column.props.showLength" :placeholder="column.props.placeholder"
								:format="column.props.format" :enablePrint="column.props.enablePrint"
								:required="column.props.required" :keyId="column.id"
								v-if="column&&column.name=='DateTimeRange'"></DateTimeRange>
							<Description @update:value="updateval($event,i,column.id)" style="width: 100%;"
								:ref="'form'+column.id" :mode="mode" :value="row[column.id]"
								:enablePrint="column.props.enablePrint" :required="column.props.required"
								:keyId="column.id" v-if="column&&column.name=='Description'">
							</Description>
							<ImageUpload @update:value="updateval($event,i,column.id)" style="width: 100%;"
								:ref="'form'+column.id" :mode="mode" :value="row[column.id]"
								:enableZip="column.props.enableZip" :maxNumber="column.props.maxNumber"
								:maxSize="column.props.maxSize" :enablePrint="column.props.enablePrint"
								:required="column.props.required" :keyId="column.id"
								v-if="column&&column.name=='ImageUpload'">
							</ImageUpload>
							<FileUpload @update:value="updateval($event,i,column.id)" style="width: 100%;"
								:ref="'form'+column.id" :mode="mode" :value="row[column.id]"
								:fileTypes="column.props.fileTypes" :onlyRead="column.props.onlyRead"
								:maxNumber="column.props.maxNumber" :maxSize="column.props.maxSize"
								:enablePrint="column.props.enablePrint" :required="column.props.required"
								:keyId="column.id" v-if="column&&column.name=='FileUpload'">
							</FileUpload>
							<Location @update:value="updateval($event,i,column.id)" style="width: 100%;"
								:ref="'form'+column.id" :mode="mode" :value="row[column.id]" :keyId="column.id"
								v-if="column&&column.name=='Location'">
							</Location>
							<DeptPicker @update:value="updateval($event,i,column.id)" style="width: 100%;"
								:ref="'form'+column.id" :mode="mode" :value="row[column.id]"
								:multiple="column.props.multiple" :enablePrint="column.props.enablePrint"
								:required="column.props.required" :keyId="column.id" @setActiveItem="setActiveItem"
								v-if="column&&column.name=='DeptPicker'"></DeptPicker>
							<UserPicker @update:value="updateval($event,i,column.id)" style="width: 100%;"
								:ref="'form'+column.id" :mode="mode" :value="row[column.id]"
								:multiple="column.props.multiple" :enablePrint="column.props.enablePrint"
								:required="column.props.required" :keyId="column.id" @setActiveItem="setActiveItem"
								v-if="column&&column.name=='UserPicker'"></UserPicker>
							<DevicPicker @update:value="updateval($event,i,column.id)" style="width: 100%;"
								:ref="'form'+column.id" :mode="mode" :value="row[column.id]"
								:synclist="column.props.synclist" :limit_product="column.props.limit_product"
								:limit="column.props.limit" :enablePrint="column.props.enablePrint"
								:required="column.props.required" :keyId="column.id" @setActiveItem="setActiveItem"
								v-if="column&&column.name=='DevicPicker'"></DevicPicker>
							<SignPanel @update:value="updateval($event,i,column.id)" style="width: 100%;"
								:ref="'form'+column.id" :mode="mode" :value="row[column.id]" :keyId="column.id"
								@setActiveItem="setActiveItem" v-if="column&&column.name=='SignPanel'"></SignPanel>
							<SpanLayout @update:value="updateval($event,i,column.id)" style="width: 100%;"
								:ref="'form'+column.id" :mode="mode" :value="_value" :items="column.props.items"
								:keyId="column.id" @setActiveItem="setActiveItem"
								v-if="column&&column.name=='SpanLayout'"></SpanLayout>
							<TableList @update:value="updateval($event,i,column.id)" style="width: 100%;"
								:ref="'form'+column.id" :mode="mode" :value="row[column.id]"
								:columns="column.props.columns" :maxSize="column.props.maxSize"
								:summaryUnit="column.props.summaryUnit" :summaryColumns="column.props.summaryColumns"
								:deductid="column.props.deductid" :showSummary="column.props.showSummary"
								:IdxColName="column.props.IdxColName" :rowLayout="column.props.rowLayout"
								:showBorder="column.props.showBorder" :enablePrint="column.props.enablePrint"
								:required="column.props.required" :keyId="column.id" @setActiveItem="setActiveItem"
								v-if="column&&column.name=='TableList'"></TableList>
							<ParamInput @update:value="updateval($event,i,column.id)" style="width: 100%;"
								:ref="'form'+column.id" :mode="mode" :value="row[column.id]"
								:formId="column.props.formId" :formType="column.props.formType"
								:enablePrint="column.props.enablePrint" :required="column.props.required"
								:keyId="column.id" @setActiveItem="setActiveItem"
								v-if="column&&column.name=='ParamInput'"></ParamInput>
						</view>
					</uni-forms-item>
					<view class="table_btn">
						<view class="btn_li active" @click="copyData(i, row)">复制</view>
						<view class="btn_line"></view>
						<view class="btn_li" @click="delRow(i, row)">删除</view>
					</view>
					<view class="sort_num">
						{{i + 1}}
					</view>
				</view>
			</uni-forms>
			<view class="form_device_add" @click.stop="addRow">
				<custom-icons iconsName="icon-tianjia" iconsSize="24rpx" iconsColor="#333333"></custom-icons>
				<view class="text">
					添加数据
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
			// FormDesignRender,
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
					return this.$store.state.flowable && this.$store.state.flowable.selectFormItem ? this.$store.state
						.flowable.selectFormItem : '';
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
			updateval(val, i, id) {
				this._value[i][id] = val
				this.$emit('update:value', this._value)
				// this.$emit("input", this._value);
			},
			setActiveItem(val) {
				this.$emit('setActiveItem', val)
			},
			selectDept(val, acitveId) {
				//选择部门
				this.$refs[`form${acitveId}`].selectDept(val, acitveId)
			},
			selectEmplee(val, acitveId) {
				//选择人员
				this.$refs[`form${acitveId}`].selectEmplee(val, acitveId)
			},
			selectDevice(val, acitveId) {
				//选择设备
				this.$refs[`form${acitveId}`].selectDevice(val, acitveId)
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
							return !this.$isNotEmpty(val);
					}
				}
				return false;
			},
			copyData(i, row) {
				this._value.push(deepCopy(row));
				this.$emit('update:value', this._value)
				// this.$emit("input", this._value);
			},
			delRow(i, row) {
				this._value.splice(i, 1);
				this.$emit('update:value', this._value)
				// this.$emit("input", this._value);
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
					console.info(row,'一行的数据');
					if (this._value == null) {
						this.$set(this, "_value", [row]);
					} else {
						this._value.push(row);
						this.$set(this, "_value", this._value);
					}
				}
				this.$emit('update:value', this._value)
				// this.$emit("input", this._value);
				this.$forceUpdate()
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
									this.$refs.promptMsg.open('请输入 ' + this.columns[i].title, 2000)
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
							try {
								let valid = await formRef[0].validate()
								if (valid) {
									success++;
								}
							} catch (err) {
								//TODO handle the exception
								// console.log("err", err);
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