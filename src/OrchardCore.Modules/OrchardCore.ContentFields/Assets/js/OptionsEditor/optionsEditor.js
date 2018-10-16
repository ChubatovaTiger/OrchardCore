function initializeOptionsEditor(elem, data, defaultValue) {

    var previouslyChecked;

    var optionsTable = {
        template: '#options-table',
        props: ['value'],
        name: 'options-table',
        data: function() {
            return {
                options: data,
                selected: defaultValue
            }
        },
        methods: {
            add: function () {
                this.options.push({ name: '', value: ''});
            },
            remove: function (index) {
                this.options.splice(index, 1);
            },
            uncheck: function (index, value) {
                if (index == previouslyChecked) {
                    $('#customRadio_' + index)[0].checked = false;
                    previouslyChecked = null;
                }
                else {
                    previouslyChecked = index;
                }

            },
            getFormattedList: function () {
                return JSON.stringify(this.options.filter(function (x) { return !IsNullOrWhiteSpace(x.name) && !IsNullOrWhiteSpace(x.value) }));
            }
        }
    };

    new Vue({
        components: {
            optionsTable: optionsTable
        },
        el: elem,
        data: {
            dragging: false
        }
    });

}

function IsNullOrWhiteSpace(str) {
    return str === null || str.match(/^ *$/) !== null;
}