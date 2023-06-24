import { Button } from "@mui/material";
import { Product } from "../../app/models/products";
import ProductList from "./ProductList";
import { useState, useEffect } from "react";

export default function Catalog() {
  const [products, setProducts] = useState<Product[]>([]);

  useEffect(() => {
    fetch("http://localhost:5000/api/products")
      .then((res) => res.json())
      .then((resp) => {
        setProducts(resp);
      });
  }, []);

  return (
    <>
      <ProductList products={products} />
    </>
  );
}
