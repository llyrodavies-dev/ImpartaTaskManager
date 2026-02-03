import { useState } from 'react';
import type { FilterRequest } from '../models/FilterRequest';
import { TaskItemStatusLabels } from '../models/enums/TaskItemStatusLabels';

type FieldOption = { value: string; label: string };
type OperatorOption = { value: number; label: string };

interface TaskFilterFormProps {
    fieldOptions: FieldOption[];
    fieldOperators: { [fieldName: string]: OperatorOption[] };
    filters: FilterRequest[];
    onAddFilter: (filter: FilterRequest) => void;
    onDeleteFilter: (index: number) => void;
}

export default function TaskFilterForm({ fieldOptions, fieldOperators, filters, onAddFilter, onDeleteFilter }: TaskFilterFormProps) {
    const [filter, setFilter] = useState<FilterRequest>({
        FieldName: fieldOptions[0]?.value || '',
        Operator: fieldOperators[fieldOptions[0]?.value]?.[0]?.value ?? 1,
        Values: [''],
    });

    const operatorOptions = fieldOperators[filter.FieldName] || [];

    const handleAdd = () => {
        if (!filter.FieldName || !filter.Values[0]) return;
        onAddFilter({ ...filter, Values: [filter.Values[0]] });
        setFilter({
            FieldName: fieldOptions[0]?.value || '',
            Operator: fieldOperators[fieldOptions[0]?.value]?.[0]?.value ?? 1,
            Values: [''],
        });
    };

    const getFieldLabel = (value: string) =>
        fieldOptions.find(f => f.value === value)?.label || value;
    // const getOperatorLabel = (value: number) =>
    //     fieldOperators[filter.FieldName]?.find(o => o.value === value)?.label || value;

    const getOperatorLabel = (value: number) => {
        for (const ops of Object.values(fieldOperators)) {
            const found = ops.find(o => o.value === value);
            if (found) return found.label;
        }
        return value;
    };

    return (
        <div className="mb-6 bg-white rounded-xl shadow p-6 w-fit" 
            style={{ 
                background: 'var(--color-white-1)', 
                marginLeft: 0, 
                }}>
            <div className="flex flex-wrap items-end gap-4">
                <div>
                    <label className="block text-sm font-medium text-blue-800 mb-1">Field</label>
                    <select
                        value={filter.FieldName}
                        onChange={e => setFilter(f => ({ ...f, FieldName: e.target.value, Operator: fieldOperators[e.target.value]?.[0]?.value ?? 1 }))}
                        className="border border-gray-300 rounded px-3 py-2 bg-gray-50 focus:outline-none focus:ring-2 focus:ring-blue-200"
                    >
                        {fieldOptions.map(opt => (
                            <option key={opt.value} value={opt.value}>{opt.label}</option>
                        ))}
                    </select>
                </div>
                <div>
                    <label className="block text-sm font-medium text-blue-800 mb-1">Operator</label>
                    <select
                        value={filter.Operator}
                        onChange={e => setFilter(f => ({ ...f, Operator: Number(e.target.value) }))}
                        className="border border-gray-300 rounded px-3 py-2 bg-gray-50 focus:outline-none focus:ring-2 focus:ring-blue-200"
                    >
                        {operatorOptions.map(opt => (
                            <option key={opt.value} value={opt.value}>{opt.label}</option>
                        ))}
                    </select>
                </div>
                <div>
                    <label className="block text-sm font-medium text-blue-800 mb-1">Value</label>
                    {filter.FieldName === 'Status' ? (
                        <select
                            value={filter.Values[0]}
                            onChange={e => setFilter(f => ({ ...f, Values: [e.target.value] }))}
                            className="border border-gray-300 rounded px-3 py-2 bg-gray-50 focus:outline-none focus:ring-2 focus:ring-blue-200"
                        >
                            <option value="">Select status</option>
                            {Object.entries(TaskItemStatusLabels)
                                .filter(([value]) => value !== "0") // Exclude "Unspecified"
                                .map(([value, label]) => (
                                    <option key={value} value={value}>{label}</option>
                                ))}
                        </select>
                    ) : (
                        <input
                            type="text"
                            value={filter.Values[0]}
                            onChange={e => setFilter(f => ({ ...f, Values: [e.target.value] }))}
                            className="border border-gray-300 rounded px-3 py-2 bg-gray-50 focus:outline-none focus:ring-2 focus:ring-blue-200"
                        />
                    )}
                </div>
                <button
                    onClick={handleAdd}
                    className="bg-blue-600 hover:bg-blue-700 text-white px-5 py-2 rounded shadow transition"
                >
                    Add Filter
                </button>
            </div>
            {filters.length > 0 && (
                <div className="mt-4">
                    <ul>
                        {filters.map((f, idx) => {
                            let valueDisplay = f.Values[0];
                            if (f.FieldName === "Status") {
                                valueDisplay = TaskItemStatusLabels[Number(f.Values[0])] || f.Values[0];
                            }
                            return (
                                <li key={idx} className="flex items-center gap-2 mb-2">
                                    <span className="px-3 py-1 bg-blue-50 rounded-lg text-blue-800 text-sm font-medium shadow">
                                        {getFieldLabel(f.FieldName)} {getOperatorLabel(f.Operator)} "<span className="font-semibold">{valueDisplay}</span>"
                                    </span>
                                    <button
                                        onClick={() => onDeleteFilter(idx)}
                                        className="ml-2 text-red-600 hover:underline text-sm"
                                        title="Delete"
                                    >
                                        Delete
                                    </button>
                                </li>
                            );
                        })}
                    </ul>
                </div>
            )}
        </div>
    );
}