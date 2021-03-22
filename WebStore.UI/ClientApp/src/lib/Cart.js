class Cart {

    constructor() {       
        if (!Cart.instance) {
            var cart = window.localStorage.getItem('cart');
            if (cart)
                this._contents = JSON.parse(cart);
            else
                this._contents = {};
        }
        return Cart.instance;
    }

    deleteItem(inventoryId) {
        delete this._contents[inventoryId];
        window.localStorage.setItem('cart', JSON.stringify(this._contents));
    }

    adjustItem(inventoryId, quantity, name, price, description) {
        var hasItemInCart = Object.keys(this._contents).includes(inventoryId.toString());
        console.log(hasItemInCart);
        console.log(Object.keys(this._contents));
        if (quantity < 0)
            return;
        this._contents[inventoryId] =
            {
                quantity: hasItemInCart ?
                    parseInt(this._contents[inventoryId].quantity) + parseInt(quantity) :
                    quantity,
                name,
                price,
                description
            };
        console.log(this._contents)
        console.log(JSON.stringify(this._contents));
        
        window.localStorage.setItem('cart', JSON.stringify(this._contents));
    }

    getItem(inventoryId) {
        if (this._contents.has(inventoryId)) {
            return JSON.parse(window.localStorage.getItem('cart'));
        } else {
            return 0;
        }
    }

    emptyCart() {        
        this._contents = {};        
        window.localStorage.setItem('cart', JSON.stringify(this._contents));
    }
}

const cart = new Cart();
Object.freeze(cart);
export default cart;