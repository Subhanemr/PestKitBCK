const cartItemHolder = document.querySelector(".cart-item-holder");
const addToCartButtons = document.querySelectorAll(".add-to-cart");
const deleteFromCartButtons = document.querySelectorAll(".delete-from-cart"); 
const cartCountElement = document.querySelector(".cartItemCount");

addToCartButtons.forEach(button =>
    button.addEventListener("click", ev => {
        ev.preventDefault();

        const href = ev.target.getAttribute("href");

        fetch(href)
            .then(res => res.text())
            .then(data => {
                cartItemHolder.innerHTML = data;
                updateCartItemCount();
            })
            .catch(error => console.error("Error fetching data:", error));
    })
);


deleteFromCartButtons.forEach(button =>
    button.addEventListener("click", ev => {
        ev.preventDefault();

        const href = ev.target.parentElement.getAttribute("href");

        fetch(href)
            .then(res => res.text())
            .then(data => {
                cartItemHolder.innerHTML = data;
                updateCartItemCount();
            })
            .catch(error => console.error("Error fetching data:", error));
    })
);

function updateCartItemCount() {
    const cartItems = document.querySelectorAll(".getCartItemCount");
    cartItems.forEach(item => {
        const countValue = item.dataset.count;
        cartCountElement.textContent = countValue;
    });

}
